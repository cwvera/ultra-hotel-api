using FluentValidation.Results;
using UltraHotel.Application.Features.Rooms.Commands;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Tests.Unit.Features.Rooms;

public class UpdateRoomCommandValidatorTests
{
    private readonly UpdateRoomCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_IsValid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new UpdateRoomCommand(Guid.NewGuid(), RoomType.Suite, 200m, 0.19m, 3, "Floor 2"));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyRoomId_IsInvalid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new UpdateRoomCommand(Guid.Empty, RoomType.Double, 100m, 0.19m, 2, null));
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_BasePriceZeroOrNegative_IsInvalid(decimal price)
    {
        ValidationResult result = await _validator.ValidateAsync(
            new UpdateRoomCommand(Guid.NewGuid(), RoomType.Double, price, 0.19m, 2, null));
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_CapacityZero_IsInvalid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new UpdateRoomCommand(Guid.NewGuid(), RoomType.Double, 100m, 0.19m, 0, null));
        Assert.False(result.IsValid);
    }
}
