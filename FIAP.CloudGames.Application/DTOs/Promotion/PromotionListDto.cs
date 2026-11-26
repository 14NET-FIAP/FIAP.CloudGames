namespace FIAP.CloudGames.Application.DTOs.Promotion;

public record PromotionListDto(
    Guid Id,
    Guid GameId,
    decimal DiscountPercentage,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    DateTime CreatedAt);
