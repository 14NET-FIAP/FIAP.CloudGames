using FIAP.CloudGames.DataBase.Context;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Repositories;

namespace FIAP.CloudGames.DataBase.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly AppDbContext _context;

    public PromotionRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Promotion> GetActivePromotions(DateTime currentDate)
    {
        return _context.Promotion
            .Where(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate)
            .ToList();
    }
}
