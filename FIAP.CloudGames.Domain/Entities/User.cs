using FIAP.CloudGames.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public TypeUserEnum TypeUser { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    public ICollection<UserLibrary> UserLibraries { get; private set; } = new List<UserLibrary>();

    public User() { }
    public User(string name, string email, string password)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = password;
        TypeUser = TypeUserEnum.User;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Update(string? name, string? email, string? passwordHash)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        if (!string.IsNullOrWhiteSpace(email))
            Email = email;

        if (!string.IsNullOrWhiteSpace(passwordHash))
            PasswordHash = passwordHash;
    }

    public void Deactivate() => IsActive = false;
}
