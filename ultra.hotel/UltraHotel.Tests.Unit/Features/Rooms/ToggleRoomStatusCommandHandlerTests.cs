using Moq;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Rooms.Commands;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Application.Features.Search.Dtos;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Tests.Unit.Features.Rooms;

public class ToggleRoomStatusCommandHandlerTests
{
    private readonly Mock<IRoomRepository> _roomRepo = new();
    private readonly Mock<IHotelRepository> _hotelRepo = new();
    private readonly Mock<IElasticsearchService> _es = new();
    private readonly ToggleRoomStatusCommandHandler _sut;

    private static readonly Guid HotelId = Guid.NewGuid();
    private static readonly Guid RoomId = Guid.NewGuid();
    private static readonly Hotel ExistingHotel = new() { Id = HotelId, Name = "H", City = "Bogotá", Address = "A", AgentEmail = "a@b.com" };
    private static readonly Room ExistingRoom = new() { Id = RoomId, HotelId = HotelId, RoomType = RoomType.Double, BasePrice = 100m, TaxRate = 0.19m, Capacity = 2 };

    public ToggleRoomStatusCommandHandlerTests()
    {
        _sut = new ToggleRoomStatusCommandHandler(_roomRepo.Object, _hotelRepo.Object, _es.Object);
    }

    [Fact]
    public async Task Handle_ExistingRoom_TogglesAndIndexes()
    {
        _roomRepo.Setup(r => r.GetByIdAsync(RoomId, It.IsAny<CancellationToken>())).ReturnsAsync(ExistingRoom);
        _hotelRepo.Setup(r => r.GetByIdAsync(HotelId, It.IsAny<CancellationToken>())).ReturnsAsync(ExistingHotel);
        _roomRepo.Setup(r => r.UpdateAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _es.Setup(e => e.IndexRoomAsync(It.IsAny<RoomIndexDocument>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _sut.Handle(new ToggleRoomStatusCommand(RoomId, false), CancellationToken.None);

        _roomRepo.Verify(r => r.UpdateAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Once);
        _es.Verify(e => e.IndexRoomAsync(It.IsAny<RoomIndexDocument>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RoomNotFound_ThrowsKeyNotFoundException()
    {
        _roomRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Room?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.Handle(new ToggleRoomStatusCommand(RoomId, true), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_HotelNotFound_ThrowsKeyNotFoundException()
    {
        _roomRepo.Setup(r => r.GetByIdAsync(RoomId, It.IsAny<CancellationToken>())).ReturnsAsync(ExistingRoom);
        _hotelRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((Hotel?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.Handle(new ToggleRoomStatusCommand(RoomId, true), CancellationToken.None));
    }
}
