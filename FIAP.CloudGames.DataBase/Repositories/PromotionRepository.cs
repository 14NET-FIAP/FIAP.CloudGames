using Dapper;
using FIAP.CloudGames.DataBase.Context;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FIAP.CloudGames.DataBase.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly AppDbContext _context;
    private readonly ISqlServerConnectionFactory _connectionFactory;

    public PromotionRepository(AppDbContext context, ISqlServerConnectionFactory sqlServerConnectionFactory)
    {
        _context = context;
        _connectionFactory = sqlServerConnectionFactory;
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

        if (ids.Count == 0)
            return Array.Empty<Promotion>();

        const string sql = """
        SELECT *
        FROM Promotion
        WHERE GameId IN @GameIds
          AND StartDate <= @CurrentDate
          AND EndDate >= @CurrentDate
          AND IsActive = 1
        """;

        using IDbConnection connection = _connectionFactory.CreateConnection();

        IEnumerable<Promotion> promotions = await connection.QueryAsync<Promotion>(
            new CommandDefinition(
                sql,
                new { GameIds = ids, CurrentDate = currentDate },
                cancellationToken: ct));

        return promotions.ToList();
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
