using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Domain.Entities;

public class Game
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }


    public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
    public ICollection<UserLibrary> UserLibraries { get; set; } = new List<UserLibrary>();


    protected Game() { }


    public Game(string title, string description, decimal price, DateTime releaseDate)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Price = price;
        ReleaseDate = releaseDate;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    // MidpointRounding.AwayFromZero reproduz o ROUND do SQL Server usado na listagem de jogos,
    // garantindo que o valor cobrado seja o mesmo exibido ao usuário.
    public decimal CalculatePriceWithDiscount(decimal discountPercentage) =>
        Math.Round(Price * (100 - discountPercentage) / 100, 2, MidpointRounding.AwayFromZero);

    public Game(Guid id, string title, string description, decimal price, DateTime releaseDate)
    {
        Id = id;
        Title = title;
        Description = description;
        Price = price;
        ReleaseDate = releaseDate;
    }
}
