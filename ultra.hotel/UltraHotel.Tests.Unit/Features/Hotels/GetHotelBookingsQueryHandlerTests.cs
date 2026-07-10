using Mapster;
using Moq;
using UltraHotel.Application.Features.Bookings.Contracts;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Hotels.Dtos;
using UltraHotel.Application.Features.Hotels.Queries;
using UltraHotel.Application.Mappings;
using UltraHotel.Domain.Entities.Bookings;

namespace UltraHotel.Tests.Unit.Features.Hotels;

public class GetHotelBookingsQueryHandlerTests
{
    private readonly Mock<IHotelRepository> _hotelRepo = new();
    private readonly Mock<IBookingRepository> _bookingRepo = new();
    private readonly GetHotelBookingsQueryHandler _sut;

    private static readonly Guid HotelId = Guid.NewGuid();

    public GetHotelBookingsQueryHandlerTests()
    {
        new MappingConfig().Register(TypeAdapterConfig.GlobalSettings);
        _sut = new GetHotelBookingsQueryHandler(_hotelRepo.Object, _bookingRepo.Object);
    }

    [Fact]
    public async Task Handle_ExistingHotel_ReturnsBookings()
    {
        Booking booking = new() { Id = Guid.NewGuid(), HotelId = HotelId, RoomId = Guid.NewGuid(), Status = BookingStatus.Confirmed, TotalPrice = 200m, CheckInDate = new DateOnly(2025, 6, 1), CheckOutDate = new DateOnly(2025, 6, 3) };
        _hotelRepo.Setup(r => r.ExistsByIdAsync(HotelId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _bookingRepo.Setup(r => r.GetByHotelIdAsync(HotelId, It.IsAny<CancellationToken>())).ReturnsAsync([booking]);

        IReadOnlyList<HotelBookingSummaryDto> result = await _sut.Handle(
            new GetHotelBookingsQuery(HotelId), CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task Handle_HotelNotFound_ThrowsKeyNotFoundException()
    {
        _hotelRepo.Setup(r => r.ExistsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.Handle(new GetHotelBookingsQuery(HotelId), CancellationToken.None));
    }
}
