using FIAP.CloudGames.Domain.Entities;

namespace FIAP.CloudGames.Application.Tests.Entities;

public class UserTests
{
    private const string Name = "Maria";
    private const string Email = "maria@email.com";
    private const string PasswordHash = "hash-original";

    [Fact]
    public void Update_WithAllValues_ReplacesAllFields()
    {
        var user = new User(Name, Email, PasswordHash);

        user.Update("Maria Silva", "maria.silva@email.com", "hash-novo");

        Assert.Equal("Maria Silva", user.Name);
        Assert.Equal("maria.silva@email.com", user.Email);
        Assert.Equal("hash-novo", user.PasswordHash);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithNullOrBlankValues_KeepsCurrentFields(string? emptyValue)
    {
        var user = new User(Name, Email, PasswordHash);

        user.Update(emptyValue, emptyValue, emptyValue);

        Assert.Equal(Name, user.Name);
        Assert.Equal(Email, user.Email);
        Assert.Equal(PasswordHash, user.PasswordHash);
    }

    [Fact]
    public void Deactivate_SetsUserAsInactive()
    {
        var user = new User(Name, Email, PasswordHash);

        user.Deactivate();

        Assert.False(user.IsActive);
    }
}
