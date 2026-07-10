using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Application.Features.Search.Dtos;

namespace UltraHotel.Infrastructure.Elasticsearch;

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchService> _logger;
    private readonly ResiliencePipeline _pipeline;
    private const string IndexPattern = "hotel_*";

    public ElasticsearchService(IOptions<ElasticsearchSettings> options, ILogger<ElasticsearchService> logger)
    {
        _logger = logger;
        ElasticsearchClientSettings settings = new(new Uri(options.Value.Url));
        _client = new ElasticsearchClient(settings);
        _pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay            = TimeSpan.FromSeconds(2),
                BackoffType      = DelayBackoffType.Exponential,
                ShouldHandle     = args => ValueTask.FromResult(args.Outcome.Exception is not null),
                OnRetry          = args =>
                {
                    _logger.LogWarning("Elasticsearch retry #{Attempt} — {Exception}",
                        args.AttemptNumber + 1, args.Outcome.Exception?.Message);
                    return default;
                }
            })
            .Build();
    }

    /// <inheritdoc/>
    public async Task IndexRoomAsync(RoomIndexDocument document, CancellationToken ct = default)
    {
        string index = $"hotel_{document.YearMonth}";
        try
        {
            await _pipeline.ExecuteAsync(async innerCt =>
            {
                IndexResponse response = await _client.IndexAsync(
                    document, i => i.Index(index).Id(document.RoomId), innerCt);

                if (!response.IsValidResponse)
                    throw new InvalidOperationException(
                        response.ElasticsearchServerError?.ToString() ?? "Index failed");
            }, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ES index failed for room {RoomId} after retries", document.RoomId);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<RoomSearchResult>> SearchRoomsAsync(
        string city, int minCapacity, CancellationToken ct = default)
    {
        try
        {
            return await _pipeline.ExecuteAsync(async innerCt =>
            {
                SearchResponse<RoomIndexDocument> response = await _client.SearchAsync<RoomIndexDocument>(s => s
                    .Indices(IndexPattern)
                    .Query(q => q
                        .Bool(b => b
                            .Filter(
                                f => f.Term(t => t.Field("city").Value(city)),
                                f => f.Range(r => r.Number(n => n.Field("capacity").Gte(minCapacity))),
                                f => f.Term(t => t.Field("is_available").Value(true)),
                                f => f.Term(t => t.Field("hotel_enabled").Value(true)),
                                f => f.Term(t => t.Field("room_enabled").Value(true))
                            )))
                    .Size(200), innerCt);

                if (!response.IsValidResponse)
                    throw new InvalidOperationException(
                        response.ElasticsearchServerError?.ToString() ?? "Search failed");

                return (IReadOnlyList<RoomSearchResult>)response.Documents
                    .Select(d => new RoomSearchResult(
                        Guid.Parse(d.RoomId),
                        Guid.Parse(d.HotelId),
                        d.HotelName,
                        d.City,
                        d.RoomType,
                        d.Capacity,
                        (decimal)d.BasePrice,
                        (decimal)d.TaxRate,
                        Math.Round((decimal)d.BasePrice * (1 + (decimal)d.TaxRate), 2),
                        d.LocationInHotel))
                    .ToList()
                    .AsReadOnly();
            }, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ES search failed after retries for city {City}", city);
            return [];
        }
    }

    /// <inheritdoc/>
    public async Task DeleteRoomAsync(Guid roomId, CancellationToken ct = default)
    {
        try
        {
            await _pipeline.ExecuteAsync(async innerCt =>
            {
                DeleteByQueryResponse response = await _client.DeleteByQueryAsync<RoomIndexDocument>(
                    IndexPattern,
                    q => q.Query(qu => qu.Term(t => t.Field("room_id").Value(roomId.ToString()))),
                    innerCt);

                if (!response.IsValidResponse)
                    throw new InvalidOperationException(
                        response.ElasticsearchServerError?.ToString() ?? "Delete failed");
            }, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ES delete failed for room {RoomId} after retries", roomId);
        }
    }
}
