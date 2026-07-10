using FluentValidation.Results;
using UltraHotel.Application.Features.Rooms.Commands;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Tests.Unit.Features.Rooms;

public class CreateRoomCommandValidatorTests
{
    private readonly CreateRoomCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_IsValid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new CreateRoomCommand(Guid.NewGuid(), RoomType.Double, 150m, 0.19m, 2, null));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyHotelId_IsInvalid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new CreateRoomCommand(Guid.Empty, RoomType.Double, 150m, 0.19m, 2, null));
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Validate_BasePriceZeroOrNegative_IsInvalid(decimal price)
    {
        ValidationResult result = await _validator.ValidateAsync(
            new CreateRoomCommand(Guid.NewGuid(), RoomType.Double, price, 0.19m, 2, null));
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public async Task Validate_TaxRateOutOfRange_IsInvalid(decimal taxRate)
    {
        ValidationResult result = await _validator.ValidateAsync(
            new CreateRoomCommand(Guid.NewGuid(), RoomType.Double, 100m, taxRate, 2, null));
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_CapacityZero_IsInvalid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new CreateRoomCommand(Guid.NewGuid(), RoomType.Suite, 100m, 0.19m, 0, null));
        Assert.False(result.IsValid);
    }
}
