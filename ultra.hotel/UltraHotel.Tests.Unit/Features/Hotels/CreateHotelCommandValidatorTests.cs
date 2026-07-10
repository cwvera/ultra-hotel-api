using FluentValidation.Results;
using UltraHotel.Application.Features.Hotels.Commands;

namespace UltraHotel.Tests.Unit.Features.Hotels;

public class CreateHotelCommandValidatorTests
{
    private readonly CreateHotelCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_IsValid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new CreateHotelCommand("Hotel XYZ", "Bogotá", "Calle 1", "Desc", "agent@test.com"));
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Bogotá", "Calle 1", "a@b.com")]
    [InlineData("Hotel", "", "Calle 1", "a@b.com")]
    [InlineData("Hotel", "Bogotá", "", "a@b.com")]
    [InlineData("Hotel", "Bogotá", "Calle 1", "")]
    [InlineData("Hotel", "Bogotá", "Calle 1", "not-an-email")]
    public async Task Validate_InvalidField_IsInvalid(string name, string city, string address, string email)
    {
        ValidationResult result = await _validator.ValidateAsync(
            new CreateHotelCommand(name, city, address, null, email));
        Assert.False(result.IsValid);
    }
}
