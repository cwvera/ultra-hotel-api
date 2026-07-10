using Mapster;
using Moq;
using UltraHotel.Application.Mappings;
using UltraHotel.Application.Features.Hotels.Commands;
using UltraHotel.Application.Features.Hotels.Contracts;
using UltraHotel.Application.Features.Hotels.Dtos;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Tests.Unit.Features.Hotels;

public class CreateHotelCommandHandlerTests
{
    private readonly Mock<IHotelRepository>    _repo = new();
    private readonly CreateHotelCommandHandler _sut;

    public CreateHotelCommandHandlerTests()
    {
        new MappingConfig().Register(TypeAdapterConfig.GlobalSettings);

        _sut = new CreateHotelCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsHotelDto()
    {
        CreateHotelCommand command = new("Hotel XYZ", "Bogotá", "Calle 1", "Desc", "agent@test.com");
        _repo.Setup(r => r.AddAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        HotelDto result = await _sut.Handle(command, CancellationToken.None);

        Assert.Equal("Hotel XYZ", result.Name);
        Assert.Equal("Bogotá", result.City);
        Assert.True(result.IsEnabled);
        _repo.Verify(r => r.AddAsync(
            It.Is<Hotel>(h => h.Name == "Hotel XYZ" && h.City == "Bogotá" && h.AgentEmail == "agent@test.com"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_HotelIsEnabledByDefault()
    {
        CreateHotelCommand command = new("H", "C", "A", null, "a@b.com");
        _repo.Setup(r => r.AddAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        HotelDto result = await _sut.Handle(command, CancellationToken.None);
        Assert.True(result.IsEnabled);
    }
}
