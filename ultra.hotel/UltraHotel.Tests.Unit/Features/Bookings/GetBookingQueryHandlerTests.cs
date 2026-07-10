using Mapster;
using Moq;
using UltraHotel.Application.Features.Bookings.Contracts;
using UltraHotel.Application.Features.Bookings.Dtos;
using UltraHotel.Application.Features.Bookings.Queries;
using UltraHotel.Application.Mappings;
using UltraHotel.Domain.Entities.Bookings;

namespace UltraHotel.Tests.Unit.Features.Bookings;

public class GetBookingQueryHandlerTests
{
    private readonly Mock<IBookingRepository> _repo = new();
    private readonly GetBookingQueryHandler _sut;

    private static readonly Guid BookingId = Guid.NewGuid();

    public GetBookingQueryHandlerTests()
    {
        new MappingConfig().Register(TypeAdapterConfig.GlobalSettings);
        _sut = new GetBookingQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ExistingBooking_ReturnsDto()
    {
        Booking booking = new()
        {
            Id = BookingId,
            RoomId = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Status = BookingStatus.Confirmed,
            TotalPrice = 300m,
            CheckInDate = new DateOnly(2025, 6, 1),
            CheckOutDate = new DateOnly(2025, 6, 4),
            EmergencyContactName = "Juan",
            EmergencyContactPhone = "555-0000"
        };
        _repo.Setup(r => r.GetByIdAsync(BookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        BookingDto result = await _sut.Handle(new GetBookingQuery(BookingId), CancellationToken.None);

        Assert.Equal(BookingId, result.Id);
        Assert.Equal(300m, result.TotalPrice);
    }

    [Fact]
    public async Task Handle_BookingNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Booking?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.Handle(new GetBookingQuery(BookingId), CancellationToken.None));
    }
}
