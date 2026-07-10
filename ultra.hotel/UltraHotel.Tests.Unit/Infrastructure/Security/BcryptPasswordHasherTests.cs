using UltraHotel.Infrastructure.Security;

namespace UltraHotel.Tests.Unit.Infrastructure.Security;

public class BcryptPasswordHasherTests
{
    private readonly BcryptPasswordHasher _sut = new();

    [Fact]
    public void Hash_ValidPassword_ReturnsBcryptFormat()
    {
        string hash = _sut.Hash("TestPassword1!");

        Assert.False(string.IsNullOrEmpty(hash));
        Assert.StartsWith("$2", hash);
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        string hash = _sut.Hash("TestPassword1!");

        Assert.True(_sut.Verify("TestPassword1!", hash));
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        string hash = _sut.Hash("TestPassword1!");

        Assert.False(_sut.Verify("WrongPassword!", hash));
    }
}
