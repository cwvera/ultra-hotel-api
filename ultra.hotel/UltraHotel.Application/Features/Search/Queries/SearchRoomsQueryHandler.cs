using MediatR;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Application.Features.Bookings.Contracts;
using UltraHotel.Application.Features.Search.Dtos;

namespace UltraHotel.Application.Features.Search.Queries;

public class SearchRoomsQueryHandler(
    IElasticsearchService elasticsearchService,
    IBookingRepository bookingRepository)
    : IRequestHandler<SearchRoomsQuery, IReadOnlyList<RoomSearchResult>>
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<RoomSearchResult>> Handle(
        SearchRoomsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<RoomSearchResult> candidates = await elasticsearchService.SearchRoomsAsync(
            request.City.ToLower(), request.Guests, cancellationToken);

        if (candidates.Count == 0)
            return [];

        IEnumerable<Guid>       candidateIds   = candidates.Select(c => c.RoomId);
        IReadOnlyList<Guid>     unavailableIds = await bookingRepository.GetUnavailableRoomIdsAsync(
            candidateIds, request.CheckIn, request.CheckOut, cancellationToken);
        HashSet<Guid>           unavailableSet = unavailableIds.ToHashSet();

        return candidates
            .Where(c => !unavailableSet.Contains(c.RoomId))
            .ToList()
            .AsReadOnly();
    }
}
