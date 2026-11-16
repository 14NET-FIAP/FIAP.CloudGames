using FIAP.CloudGames.Domain.Entities;

namespace FIAP.CloudGames.Domain.Repositories;

public interface IPromotionRepository
{
    void CreatePromotion(Promotion promotion);
    IEnumerable<Promotion> GetActivePromotions(DateTime currentDate);
    Task<IReadOnlyList<Promotion>> GetActivePromotionsByGameIdsAsync(IEnumerable<Guid> gameIds,DateTime currentDate,CancellationToken ct = default);
    void DesactivePromotion(Guid id);
    void UpdateAsync(Promotion updatedPromotion);
    bool VerifyIfExist(Guid id);
}
