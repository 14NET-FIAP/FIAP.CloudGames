using FIAP.CloudGames.Domain.Exceptions;

namespace FIAP.CloudGames.Domain.Entities;

public class Promotion
{
    public const decimal MinDiscountPercentage = 0m;
    public const decimal MaxDiscountPercentage = 100m;

    public const string InvalidDiscountMessage = "O desconto deve estar entre 0 e 100.";
    public const string InvalidPeriodMessage = "A data final não pode ser anterior à data inicial.";

    public Guid Id { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid GameId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    public Game Game { get; private set; } = null!;

    protected Promotion() { }

    public Promotion(decimal discountPercentage, DateTime startDate, DateTime endDate, Guid gameId)
    {
        EnsureValidDiscount(discountPercentage);
        EnsureValidPeriod(startDate, endDate);

        Id = Guid.NewGuid();
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
        GameId = gameId;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Update(decimal discountPercentage, DateTime startDate, DateTime endDate, Guid gameId)
    {
        EnsureValidDiscount(discountPercentage);
        EnsureValidPeriod(startDate, endDate);

        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
        GameId = gameId;
    }

    public void Deactivate() => IsActive = false;

    public static bool IsValidDiscount(decimal discountPercentage) =>
        discountPercentage is >= MinDiscountPercentage and <= MaxDiscountPercentage;

    // Datas iguais são aceitas: promoção válida durante um único instante/dia.
    public static bool IsValidPeriod(DateTime startDate, DateTime endDate) => endDate >= startDate;

    private static void EnsureValidDiscount(decimal discountPercentage)
    {
        if (!IsValidDiscount(discountPercentage))
            throw new DomainException(InvalidDiscountMessage);
    }

    private static void EnsureValidPeriod(DateTime startDate, DateTime endDate)
    {
        if (!IsValidPeriod(startDate, endDate))
            throw new DomainException(InvalidPeriodMessage);
    }
}
