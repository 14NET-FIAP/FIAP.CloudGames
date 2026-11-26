namespace FIAP.CloudGames.Domain.ReadModels;

public sealed class PromotionListRow
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
