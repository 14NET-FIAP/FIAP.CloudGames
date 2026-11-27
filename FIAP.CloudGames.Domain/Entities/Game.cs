using FIAP.CloudGames.Domain.Exceptions;

namespace FIAP.CloudGames.Domain.Entities;

public class Game
{
    public const decimal MinPrice = 0m;

    public const string TitleRequiredMessage = "O título é obrigatório.";
    public const string NegativePriceMessage = "O preço do jogo não pode ser negativo.";

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public DateTime? ReleaseDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    public ICollection<Promotion> Promotions { get; private set; } = new List<Promotion>();
    public ICollection<UserLibrary> UserLibraries { get; private set; } = new List<UserLibrary>();

    protected Game() { }

    public Game(string title, string description, decimal price, DateTime releaseDate)
    {
        EnsureValidTitle(title);
        EnsureValidPrice(price);

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Price = price;
        ReleaseDate = releaseDate;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    // Atualização parcial: valores nulos ou em branco mantêm o dado atual.
    public void Update(string? title, string? description, decimal? price, DateTime? releaseDate)
    {
        if (price.HasValue)
            EnsureValidPrice(price.Value);

        if (!string.IsNullOrWhiteSpace(title))
            Title = title;

        if (!string.IsNullOrWhiteSpace(description))
            Description = description;

        if (price.HasValue)
            Price = price.Value;

        if (releaseDate.HasValue)
            ReleaseDate = releaseDate;
    }

    public void Deactivate() => IsActive = false;

    public decimal CalculatePriceWithDiscount(decimal discountPercentage) =>
        Math.Round(Price * (100 - discountPercentage) / 100, 2, MidpointRounding.AwayFromZero);

    private static void EnsureValidTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException(TitleRequiredMessage);
    }

    private static void EnsureValidPrice(decimal price)
    {
        if (price < MinPrice)
            throw new DomainException(NegativePriceMessage);
    }
}
