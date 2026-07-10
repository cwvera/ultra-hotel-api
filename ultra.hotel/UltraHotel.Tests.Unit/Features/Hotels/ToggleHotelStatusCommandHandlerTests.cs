using Moq;
using UltraHotel.Application.Features.Hotels.Commands;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Application.Features.Search.Dtos;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Tests.Unit.Features.Hotels;

public class ToggleHotelStatusCommandHandlerTests
{
    private readonly Mock<IHotelRepository> _hotelRepo = new();
    private readonly Mock<IRoomRepository> _roomRepo = new();
    private readonly Mock<IElasticsearchService> _es = new();
    private readonly ToggleHotelStatusCommandHandler _sut;

    private static readonly Guid HotelId = Guid.NewGuid();
    private static readonly Hotel EnabledHotel = new() { Id = HotelId, Name = "H", City = "Bogotá", Address = "A", AgentEmail = "a@b.com", IsEnabled = true };

    public ToggleHotelStatusCommandHandlerTests()
    {
        _sut = new ToggleHotelStatusCommandHandler(_hotelRepo.Object, _roomRepo.Object, _es.Object);
    }

    [Fact]
    public async Task Handle_ExistingHotel_TogglesAndIndexesRooms()
    {
        Room room = new() { Id = Guid.NewGuid(), HotelId = HotelId, RoomType = RoomType.Double, BasePrice = 100m, TaxRate = 0.19m, Capacity = 2 };
        _hotelRepo.Setup(r => r.GetByIdAsync(HotelId, It.IsAny<CancellationToken>())).ReturnsAsync(EnabledHotel);
        _hotelRepo.Setup(r => r.UpdateAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _roomRepo.Setup(r => r.GetByHotelIdAsync(HotelId, It.IsAny<CancellationToken>())).ReturnsAsync([room]);
        _es.Setup(e => e.IndexRoomAsync(It.IsAny<RoomIndexDocument>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _sut.Handle(new ToggleHotelStatusCommand(HotelId, false), CancellationToken.None);

        _hotelRepo.Verify(r => r.UpdateAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>()), Times.Once);
        _es.Verify(e => e.IndexRoomAsync(It.IsAny<RoomIndexDocument>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_HotelNotFound_ThrowsKeyNotFoundException()
    {
        _hotelRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((Hotel?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.Handle(new ToggleHotelStatusCommand(HotelId, true), CancellationToken.None));
    }
}
