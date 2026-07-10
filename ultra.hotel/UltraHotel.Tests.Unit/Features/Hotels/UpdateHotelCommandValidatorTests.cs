using FluentValidation.Results;
using UltraHotel.Application.Features.Hotels.Commands;

namespace UltraHotel.Tests.Unit.Features.Hotels;

public class UpdateHotelCommandValidatorTests
{
    private readonly UpdateHotelCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_IsValid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new UpdateHotelCommand(Guid.NewGuid(), "Hotel XYZ", "Bogotá", "Calle 1", null));
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Bogotá", "Calle 1")]
    [InlineData("Hotel", "", "Calle 1")]
    [InlineData("Hotel", "Bogotá", "")]
    public async Task Validate_MissingRequiredField_IsInvalid(string name, string city, string address)
    {
        ValidationResult result = await _validator.ValidateAsync(
            new UpdateHotelCommand(Guid.NewGuid(), name, city, address, null));
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyHotelId_IsInvalid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new UpdateHotelCommand(Guid.Empty, "H", "C", "A", null));
        Assert.False(result.IsValid);
    }
}
