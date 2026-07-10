using Mapster;
using Moq;
using UltraHotel.Application.Features.Hotels.Commands;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Hotels.Dtos;
using UltraHotel.Application.Mappings;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Tests.Unit.Features.Hotels;

public class UpdateHotelCommandHandlerTests
{
    private readonly Mock<IHotelRepository> _repo = new();
    private readonly UpdateHotelCommandHandler _sut;

    private static readonly Guid HotelId = Guid.NewGuid();

    public UpdateHotelCommandHandlerTests()
    {
        new MappingConfig().Register(TypeAdapterConfig.GlobalSettings);
        _sut = new UpdateHotelCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ExistingHotel_ReturnsUpdatedDto()
    {
        Hotel hotel = new() { Id = HotelId, Name = "Old", City = "Bogotá", Address = "Old St", AgentEmail = "a@b.com" };
        _repo.Setup(r => r.GetByIdAsync(HotelId, It.IsAny<CancellationToken>())).ReturnsAsync(hotel);
        _repo.Setup(r => r.UpdateAsync(hotel, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        HotelDto result = await _sut.Handle(
            new UpdateHotelCommand(HotelId, "New Name", "Medellín", "New St", "Nice hotel"),
            CancellationToken.None);

        Assert.Equal("New Name", result.Name);
        Assert.Equal("Medellín", result.City);
        _repo.Verify(r => r.UpdateAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_HotelNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Hotel?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.Handle(new UpdateHotelCommand(HotelId, "N", "C", "A", null), CancellationToken.None));
    }
}
