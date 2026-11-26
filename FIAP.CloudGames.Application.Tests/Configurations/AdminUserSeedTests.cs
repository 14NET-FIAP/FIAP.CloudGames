using FIAP.CloudGames.DataBase.Configurations;
using FIAP.CloudGames.Domain.ValueObjects;
using FIAP.CloudGames.Infra.Security;

namespace FIAP.CloudGames.Application.Tests.Configurations;

public class AdminUserSeedTests
{
    // Senha documentada no README; o código de produção guarda apenas o hash.
    private const string DocumentedPassword = "Admin@123";

    [Fact]
    public void PasswordHash_WithDocumentedPassword_MatchesSecurityHelperHash()
    {
        Assert.Equal(AdminUserSeed.PasswordHash, SecurityHelper.HashPassword(DocumentedPassword));
    }

    [Fact]
    public void DocumentedPassword_WhenValidated_MeetsPasswordPolicy()
    {
        Assert.Null(Record.Exception(() => PasswordValueObject.IsValid(DocumentedPassword)));
    }
}
