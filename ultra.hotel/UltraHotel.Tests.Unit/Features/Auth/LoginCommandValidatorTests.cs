using FluentValidation.Results;
using UltraHotel.Application.Features.Auth.Commands;

namespace UltraHotel.Tests.Unit.Features.Auth;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_IsValid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new LoginCommand("user@test.com", "Secret123!"));
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Secret123!")]
    [InlineData("not-an-email", "Secret123!")]
    [InlineData("user@test.com", "")]
    public async Task Validate_InvalidFields_IsInvalid(string email, string password)
    {
        ValidationResult result = await _validator.ValidateAsync(new LoginCommand(email, password));
        Assert.False(result.IsValid);
    }
}
