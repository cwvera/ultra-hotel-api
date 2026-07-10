using FluentValidation.Results;
using UltraHotel.Application.Features.Auth.Commands;
using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.Tests.Unit.Features.Auth;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_IsValid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new RegisterCommand("user@test.com", "Secret123!", Role.Traveler));
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Secret123!")]
    [InlineData("not-an-email", "Secret123!")]
    [InlineData("user@test.com", "")]
    [InlineData("user@test.com", "123")]
    public async Task Validate_InvalidFields_IsInvalid(string email, string password)
    {
        ValidationResult result = await _validator.ValidateAsync(
            new RegisterCommand(email, password, Role.Traveler));
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_AdminRole_IsInvalid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new RegisterCommand("user@test.com", "Secret123!", Role.Admin));
        Assert.False(result.IsValid);
    }
}
