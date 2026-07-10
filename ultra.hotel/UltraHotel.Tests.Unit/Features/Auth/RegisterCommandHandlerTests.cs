using Mapster;
using Moq;
using UltraHotel.Application.Mappings;
using UltraHotel.Application.Features.Auth.Commands;
using UltraHotel.Application.Features.Auth.Contracts;
using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.Tests.Unit.Features.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IPasswordHasher> _hasher   = new();
    private readonly RegisterCommandHandler _sut;

    public RegisterCommandHandlerTests()
    {
        new MappingConfig().Register(TypeAdapterConfig.GlobalSettings);

        _sut = new RegisterCommandHandler(_userRepo.Object, _hasher.Object);
    }

    [Fact]
    public async Task Handle_NewUser_PersistsUser()
    {
        _userRepo.Setup(r => r.ExistsByEmailAsync("new@test.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);
        _hasher.Setup(h => h.Hash("Pass123!")).Returns("hashed");

        await _sut.Handle(new RegisterCommand("new@test.com", "Pass123!", Role.Traveler), CancellationToken.None);

        _userRepo.Verify(r => r.AddAsync(
            It.Is<User>(u => u.Email == "new@test.com" && u.PasswordHash == "hashed" && u.Role == Role.Traveler),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperation()
    {
        _userRepo.Setup(r => r.ExistsByEmailAsync("dup@test.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.Handle(new RegisterCommand("dup@test.com", "Pass1!", Role.Traveler), CancellationToken.None));
    }
}
