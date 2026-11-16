using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Domain.Entities;

public class Promotion
{
    [Key]
    public Guid Id { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public Guid GameId { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public Promotion() {}

    public Promotion(decimal discountPercentage, DateTime startDate, DateTime endDate, Guid gameId)
    {
        Id = Guid.NewGuid();
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
        GameId = gameId;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }
}
