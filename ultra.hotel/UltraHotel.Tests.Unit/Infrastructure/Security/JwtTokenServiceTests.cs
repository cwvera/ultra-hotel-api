using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using UltraHotel.Domain.Entities.Identity;
using UltraHotel.Infrastructure.Security;

namespace UltraHotel.Tests.Unit.Infrastructure.Security;

public class JwtTokenServiceTests
{
    private readonly JwtTokenService _sut;

    public JwtTokenServiceTests()
    {
        JwtSettings settings = new()
        {
            SecretKey = "UltraHotelTest_SuperSecretKey_MustBe32Chars!",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpirationMinutes = 60
        };
        _sut = new JwtTokenService(Options.Create(settings));
    }

    [Fact]
    public void GenerateToken_ValidUser_ReturnsNonEmptyString()
    {
        User user = new() { Id = Guid.NewGuid(), Email = "agent@test.com", Role = Role.Agent };

        string token = _sut.GenerateToken(user);

        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact]
    public void GenerateToken_ValidUser_ContainsEmailClaim()
    {
        User user = new() { Id = Guid.NewGuid(), Email = "agent@test.com", Role = Role.Agent };

        string token = _sut.GenerateToken(user);
        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Contains(jwt.Claims, c => c.Value == "agent@test.com");
    }

    [Fact]
    public void GenerateToken_ValidUser_ContainsRoleClaim()
    {
        User user = new() { Id = Guid.NewGuid(), Email = "admin@test.com", Role = Role.Admin };

        string token = _sut.GenerateToken(user);
        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Contains(jwt.Claims, c => c.Value == "ADMIN");
    }

    [Fact]
    public void GenerateToken_ValidUser_HasCorrectIssuer()
    {
        User user = new() { Id = Guid.NewGuid(), Email = "u@test.com", Role = Role.Traveler };

        string token = _sut.GenerateToken(user);
        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("test-issuer", jwt.Issuer);
    }
}
