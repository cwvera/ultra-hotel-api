using Mapster;
using Moq;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Rooms.Commands;
using UltraHotel.Application.Features.Rooms.Contracts;
using UltraHotel.Application.Features.Rooms.Dtos;
using UltraHotel.Application.Features.Search.Contracts;
using UltraHotel.Application.Features.Search.Dtos;
using UltraHotel.Application.Mappings;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Tests.Unit.Features.Rooms;

public class CreateRoomCommandHandlerTests
{
    private readonly Mock<IHotelRepository> _hotelRepo = new();
    private readonly Mock<IRoomRepository> _roomRepo = new();
    private readonly Mock<IElasticsearchService> _es = new();
    private readonly CreateRoomCommandHandler _sut;

    private static readonly Guid HotelId = Guid.NewGuid();
    private static readonly Hotel ExistingHotel = new() { Id = HotelId, Name = "H", City = "Bogotá", Address = "A", AgentEmail = "a@b.com", IsEnabled = true };

    public CreateRoomCommandHandlerTests()
    {
        new MappingConfig().Register(TypeAdapterConfig.GlobalSettings);
        _sut = new CreateRoomCommandHandler(_hotelRepo.Object, _roomRepo.Object, _es.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsRoomDto()
    {
        _hotelRepo.Setup(r => r.GetByIdAsync(HotelId, It.IsAny<CancellationToken>())).ReturnsAsync(ExistingHotel);
        _roomRepo.Setup(r => r.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _es.Setup(e => e.IndexRoomAsync(It.IsAny<RoomIndexDocument>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        RoomDto result = await _sut.Handle(
            new CreateRoomCommand(HotelId, RoomType.Double, 150m, 0.19m, 2, "Floor 3"),
            CancellationToken.None);

        Assert.Equal("DOUBLE", result.RoomType);
        Assert.Equal(150m, result.BasePrice);
        _es.Verify(e => e.IndexRoomAsync(It.IsAny<RoomIndexDocument>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_HotelNotFound_ThrowsKeyNotFoundException()
    {
        _hotelRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((Hotel?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.Handle(new CreateRoomCommand(HotelId, RoomType.Suite, 200m, 0.19m, 1, null), CancellationToken.None));
    }
}
