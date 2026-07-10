using Moq;
using UltraHotel.Application.Features.Auth.Commands;
using UltraHotel.Application.Features.Auth.Contracts;
using UltraHotel.Application.Features.Auth.Dtos;
using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.Tests.Unit.Features.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo     = new();
    private readonly Mock<ITokenService>   _tokenService = new();
    private readonly Mock<IPasswordHasher> _hasher       = new();
    private readonly LoginCommandHandler   _sut;

    public LoginCommandHandlerTests()
    {
        _sut = new LoginCommandHandler(_userRepo.Object, _tokenService.Object, _hasher.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        User user = new() { Id = Guid.NewGuid(), Email = "agent@test.com", PasswordHash = "hashed", Role = Role.Agent };
        _userRepo.Setup(r => r.FindByEmailAsync("agent@test.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("Secret1!", "hashed")).Returns(true);
        _tokenService.Setup(t => t.GenerateToken(user)).Returns("jwt-token");

        LoginResponse result = await _sut.Handle(
            new LoginCommand("agent@test.com", "Secret1!"), CancellationToken.None);

        Assert.Equal("jwt-token", result.Token);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorized()
    {
        _userRepo.Setup(r => r.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.Handle(new LoginCommand("x@x.com", "pass"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorized()
    {
        User user = new() { Id = Guid.NewGuid(), Email = "agent@test.com", PasswordHash = "hashed", Role = Role.Agent };
        _userRepo.Setup(r => r.FindByEmailAsync("agent@test.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);
        _hasher.Setup(h => h.Verify(It.IsAny<string>(), "hashed")).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.Handle(new LoginCommand("agent@test.com", "wrong"), CancellationToken.None));
    }
}
