using FIAP.CloudGames.Domain.Entities;

namespace FIAP.CloudGames.Domain.Repositories;

public interface IPromotionRepository
{
    IEnumerable<Promotion> GetActivePromotions(DateTime currentDate);
}
