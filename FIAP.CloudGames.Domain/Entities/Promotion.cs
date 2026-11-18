using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIAP.CloudGames.Domain.Entities;

public class Promotion
{
    public Guid Id { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid GameId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public Game Game { get; set; } = null!;

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
