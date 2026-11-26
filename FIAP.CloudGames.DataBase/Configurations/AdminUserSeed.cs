namespace FIAP.CloudGames.DataBase.Configurations;

// Valores fixos: gerados dinamicamente, cada "migrations add" detectaria mudança no seed e criaria uma migration nova.
public static class AdminUserSeed
{
    public static readonly Guid Id = new("3f2c9a1e-7b4d-4e8a-9c61-2d5f0b8e7a13");
    public static readonly DateTime CreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public const string Name = "Administrador";
    public const string Email = "admin@fiapcloudgames.com";

    // Só o hash fica no código de produção; ele precisa bater com SecurityHelper.HashPassword para o login atual funcionar.
    public const string PasswordHash = "6G94qKPK8LYNjnTllCqm2G3BUM08AzOK7yW30tfjrMc=";
}
