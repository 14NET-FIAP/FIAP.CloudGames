using Dapper;
using FIAP.CloudGames.DataBase.Context;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;

namespace FIAP.CloudGames.DataBase.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly AppDbContext _context;
    private readonly ISqlServerConnectionFactory _connectionFactory;
    private readonly ILogger<PromotionRepository> _logger;

    public PromotionRepository(AppDbContext context, ISqlServerConnectionFactory sqlServerConnectionFactory, ILogger<PromotionRepository> logger)
    {
        _context = context;
        _connectionFactory = sqlServerConnectionFactory;
        _logger = logger;
    }

    public void CreatePromotion(Promotion promotion)
    {
        _context.Promotion.Add(promotion);
        _context.SaveChanges();
    }

    public async Task<IReadOnlyList<PromotionListRow>> GetAllPromotionsAsync(CancellationToken ct = default)
    {
        const string sql = """
        SELECT
            Id,
            GameId,
            DiscountPercentage,
            StartDate,
            EndDate,
            IsActive,
            CreatedAt
        FROM Promotion
        ORDER BY IsActive DESC, CreatedAt DESC;
        """;

        using IDbConnection connection = _connectionFactory.CreateConnection();
        long startTimestamp = Stopwatch.GetTimestamp();

        IEnumerable<PromotionListRow> promotions = await connection.QueryAsync<PromotionListRow>(
            new CommandDefinition(sql, cancellationToken: ct));

        List<PromotionListRow> promotionRows = promotions.AsList();
        LogDapperCommand(nameof(GetAllPromotionsAsync), sql, promotionRows.Count, startTimestamp);

        return promotionRows;
    }

    public IEnumerable<Promotion> GetActivePromotions(DateTime currentDate)
    {
        return _context.Promotion
            .Where(promotion => promotion.StartDate <= currentDate && promotion.EndDate >= currentDate && promotion.IsActive)
            .ToList();
    }

    public async Task<IReadOnlyList<Promotion>> GetActivePromotionsByGameIdsAsync(
    IEnumerable<Guid> gameIds,
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
        long startTimestamp = Stopwatch.GetTimestamp();

        IEnumerable<Promotion> promotions = await connection.QueryAsync<Promotion>(
            new CommandDefinition(
                sql,
                new { GameIds = ids, CurrentDate = DateTime.UtcNow },
                cancellationToken: ct));

        List<Promotion> activePromotions = promotions.ToList();
        LogDapperCommand(nameof(GetActivePromotionsByGameIdsAsync), sql, activePromotions.Count, startTimestamp);

        return activePromotions;
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

    private void LogDapperCommand(string operation, string sql, int rowCount, long startTimestamp)
    {
        _logger.LogInformation("Dapper executou {Operation} em {ElapsedMilliseconds} ms ({RowCount} linhas). SQL: {Sql}",
            operation,
            Math.Round(Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds, 2),
            rowCount,
            sql);
    }
}
