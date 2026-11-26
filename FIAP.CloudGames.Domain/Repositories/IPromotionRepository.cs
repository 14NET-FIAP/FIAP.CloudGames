using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;

namespace FIAP.CloudGames.Domain.Repositories;

public interface IPromotionRepository
{
    void CreatePromotion(Promotion promotion);
    Task<IReadOnlyList<PromotionListRow>> GetAllPromotionsAsync(CancellationToken ct = default);
    IEnumerable<Promotion> GetActivePromotions(DateTime currentDate);
    Task<IReadOnlyList<Promotion>> GetActivePromotionsByGameIdsAsync(IEnumerable<Guid> gameIds,CancellationToken ct = default);
    Promotion? GetPromotionById(Guid id);
    void UpdatePromotion(Promotion promotion);
    bool VerifyIfExist(Guid id);
}
