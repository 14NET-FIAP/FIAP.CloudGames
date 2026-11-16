using FIAP.CloudGames.DataBase.Context;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FIAP.CloudGames.DataBase.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly AppDbContext _context;

    public PromotionRepository(AppDbContext context)
    {
        _context = context;
    }

    public void CreatePromotion(Promotion promotion)
    {
        _context.Promotion.Add(promotion);
        _context.SaveChanges();
    }

    public IEnumerable<Promotion> GetActivePromotions(DateTime currentDate)
    {
        return _context.Promotion
            .Where(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate && promotion.IsActive)
            .ToList();
    }

    public async Task<IReadOnlyList<Promotion>> GetActivePromotionsByGameIdsAsync(
    IEnumerable<Guid> gameIds,
    DateTime currentDate,
    CancellationToken ct = default)
    {
        var ids = gameIds.ToList();

        return await _context.Promotion
            .AsNoTracking()
            .Where(p => ids.Contains(p.GameId)
                     && p.StartDate <= currentDate
                     && p.EndDate >= currentDate
                     && p.IsActive)
            .ToListAsync(ct);
    }

    public void DesactivePromotion(Guid id)
    {
        var promotion = _context.Promotion.FirstOrDefault(p => p.Id == id);
        if (promotion == null)
            throw new KeyNotFoundException($"Promotion {id} not found.");
        promotion.IsActive = false;
        _context.SaveChanges();
    }

    public void UpdateAsync(Promotion updatedPromotion)
    {
        var existingPromotion = _context.Promotion.FirstOrDefault(p => p.Id == updatedPromotion.Id);
        if (existingPromotion == null)
            throw new KeyNotFoundException($"Promotion {updatedPromotion.Id} not found.");
       
        
        existingPromotion.DiscountPercentage = updatedPromotion.DiscountPercentage;
        existingPromotion.StartDate = updatedPromotion.StartDate;
        existingPromotion.EndDate = updatedPromotion.EndDate;
        existingPromotion.GameId = updatedPromotion.GameId;


        _context.SaveChanges();
    }

    public bool VerifyIfExist(Guid id)
    {
        return _context.Promotion.Any(promotion => promotion.Id == id);
    }
}
