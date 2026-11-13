using FIAP.CloudGames.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public TypeUserEnum TypeUser { get; set; }
    public List<OwnedGame> GamesLibrary { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public User() { }
    public User(string name, string email, string password)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = password;
        TypeUser = TypeUserEnum.Admin;
        CreatedAt = DateTime.UtcNow;
        GamesLibrary = new List<OwnedGame>();
        IsActive = true;
    }
}
