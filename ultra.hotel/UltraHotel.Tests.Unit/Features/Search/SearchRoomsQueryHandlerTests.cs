using Moq;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Application.Features.Bookings.Contracts;
using UltraHotel.Application.Features.Search.Dtos;
using UltraHotel.Application.Features.Search.Queries;

namespace UltraHotel.Tests.Unit.Features.Search;

public class SearchRoomsQueryHandlerTests
{
    private readonly Mock<IElasticsearchService> _es          = new();
    private readonly Mock<IBookingRepository>    _bookingRepo = new();
    private readonly SearchRoomsQueryHandler     _sut;

    private static readonly DateOnly CheckIn  = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
    private static readonly DateOnly CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));

    public SearchRoomsQueryHandlerTests()
    {
        _sut = new SearchRoomsQueryHandler(_es.Object, _bookingRepo.Object);
    }

    [Fact]
    public async Task Handle_NoCandidates_ReturnsEmpty()
    {
        _es.Setup(e => e.SearchRoomsAsync("bogotá", 2, It.IsAny<CancellationToken>()))
           .ReturnsAsync([]);

        IReadOnlyList<RoomSearchResult> result = await _sut.Handle(
            new SearchRoomsQuery("bogotá", CheckIn, CheckOut, 2), CancellationToken.None);

        Assert.Empty(result);
        _bookingRepo.Verify(r => r.GetUnavailableRoomIdsAsync(
            It.IsAny<IEnumerable<Guid>>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_AllCandidatesAvailable_ReturnsAll()
    {
        Guid roomId = Guid.NewGuid();
        IReadOnlyList<RoomSearchResult> candidates =
            [new(roomId, Guid.NewGuid(), "Hotel A", "bogotá", "DOUBLE", 2, 100m, 0.19m, 119m, null)];

        _es.Setup(e => e.SearchRoomsAsync("bogotá", 2, It.IsAny<CancellationToken>()))
           .ReturnsAsync(candidates);
        _bookingRepo.Setup(r => r.GetUnavailableRoomIdsAsync(
            It.IsAny<IEnumerable<Guid>>(), CheckIn, CheckOut, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<Guid>)[]);

        IReadOnlyList<RoomSearchResult> result = await _sut.Handle(
            new SearchRoomsQuery("bogotá", CheckIn, CheckOut, 2), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(roomId, result[0].RoomId);
    }

    [Fact]
    public async Task Handle_UnavailableRoomFiltered_ExcludesIt()
    {
        Guid roomId = Guid.NewGuid();
        IReadOnlyList<RoomSearchResult> candidates =
            [new(roomId, Guid.NewGuid(), "Hotel A", "bogotá", "DOUBLE", 2, 100m, 0.19m, 119m, null)];

        _es.Setup(e => e.SearchRoomsAsync("bogotá", 2, It.IsAny<CancellationToken>()))
           .ReturnsAsync(candidates);
        _bookingRepo.Setup(r => r.GetUnavailableRoomIdsAsync(
            It.IsAny<IEnumerable<Guid>>(), CheckIn, CheckOut, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<Guid>)[roomId]);

        IReadOnlyList<RoomSearchResult> result = await _sut.Handle(
            new SearchRoomsQuery("bogotá", CheckIn, CheckOut, 2), CancellationToken.None);

        Assert.Empty(result);
    }
}
