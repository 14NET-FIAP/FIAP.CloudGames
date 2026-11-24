using FIAP.CloudGames.Domain.Enums;

namespace FIAP.CloudGames.Domain.ReadModels;

public sealed class UserListRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TypeUserEnum TypeUser { get; set; }
    public DateTime CreatedAt { get; set; }
}
