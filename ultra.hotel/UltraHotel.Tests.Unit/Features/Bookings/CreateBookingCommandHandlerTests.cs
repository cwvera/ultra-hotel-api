using Mapster;
using MediatR;
using Moq;
using UltraHotel.Application.Features.Bookings.Events;
using UltraHotel.Application.Mappings;
using UltraHotel.Application.Features.Bookings.Commands;
using UltraHotel.Application.Features.Bookings.Contracts;
using UltraHotel.Application.Features.Bookings.Dtos;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Domain.Entities.Bookings;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Tests.Unit.Features.Bookings;

public class CreateBookingCommandHandlerTests
{
    private readonly Mock<IRoomRepository>    _roomRepo    = new();
    private readonly Mock<IHotelRepository>   _hotelRepo   = new();
    private readonly Mock<IBookingRepository> _bookingRepo = new();
    private readonly Mock<IPublisher>         _publisher   = new();
    private readonly CreateBookingCommandHandler _sut;

    private static readonly DateOnly CheckIn  = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
    private static readonly DateOnly CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
    private static readonly Guid HotelId      = Guid.NewGuid();
    private static readonly Guid RoomId       = Guid.NewGuid();

    private static readonly Room  EnabledRoom  = new() { Id = RoomId, HotelId = HotelId, RoomType = RoomType.Double, BasePrice = 100m, TaxRate = 0.19m, Capacity = 2, IsEnabled = true };
    private static readonly Hotel EnabledHotel = new() { Id = HotelId, Name = "H", City = "Bogotá", Address = "A", AgentEmail = "agent@test.com", IsEnabled = true };

    public CreateBookingCommandHandlerTests()
    {
        new MappingConfig().Register(TypeAdapterConfig.GlobalSettings);

        _sut = new CreateBookingCommandHandler(
            _roomRepo.Object, _hotelRepo.Object, _bookingRepo.Object, _publisher.Object);
    }

    private static CreateBookingCommand BuildCommand() => new(
        RoomId, CheckIn, CheckOut, "Emergency Name", "555-0001",
        [new GuestRequest("Juan", "Pérez", new DateOnly(1990, 1, 1),
            Gender.Male, DocumentType.Passport, "AB123456", "juan@test.com", "555-1234")]);

    [Fact]
    public async Task Handle_AvailableRoom_CreatesBookingAndPublishesEvent()
    {
        _roomRepo.Setup(r => r.GetByIdAsync(RoomId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(EnabledRoom);
        _hotelRepo.Setup(r => r.GetByIdAsync(HotelId, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(EnabledHotel);
        _bookingRepo.Setup(r => r.IsRoomAvailableAsync(RoomId, CheckIn, CheckOut, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);
        _bookingRepo.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        BookingDto result = await _sut.Handle(BuildCommand(), CancellationToken.None);

        Assert.Equal("Confirmed", result.Status);
        Assert.Single(result.Guests);
        _publisher.Verify(p => p.Publish(It.IsAny<BookingConfirmedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RoomNotFound_ThrowsKeyNotFoundException()
    {
        _roomRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Room?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.Handle(BuildCommand(), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RoomDisabled_ThrowsInvalidOperation()
    {
        Room disabledRoom = new() { Id = RoomId, HotelId = HotelId, RoomType = RoomType.Suite, BasePrice = 200m, TaxRate = 0.19m, Capacity = 2, IsEnabled = false };
        _roomRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(disabledRoom);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.Handle(BuildCommand(), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RoomUnavailableForDates_ThrowsInvalidOperation()
    {
        _roomRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(EnabledRoom);
        _hotelRepo.Setup(r => r.GetByIdAsync(HotelId, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(EnabledHotel);
        _bookingRepo.Setup(r => r.IsRoomAvailableAsync(RoomId, CheckIn, CheckOut, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.Handle(BuildCommand(), CancellationToken.None));
    }
}
