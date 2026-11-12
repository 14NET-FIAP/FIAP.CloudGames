using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Domain.Entities;

public class Promotion
{
    [Key]
    public Guid Id { get; private set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public Guid GameId { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
