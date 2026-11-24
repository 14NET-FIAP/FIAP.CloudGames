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

public class GameRepository : IGameRepository
{
    private readonly AppDbContext _context;
    private readonly ISqlServerConnectionFactory _connectionFactory;
    private readonly ILogger<GameRepository> _logger;
    public GameRepository(AppDbContext context, ISqlServerConnectionFactory sqlServerConnectionFactory, ILogger<GameRepository> logger)
    {
        _context = context;
        _connectionFactory = sqlServerConnectionFactory;
        _logger = logger;
    }

    public void CreateGame(Game game)
    {
        _context.Game.Add(game);
        _context.SaveChanges();
    }

    public IEnumerable<Game> GetAllActivesGames()
    {
        var games = _context.Game.Where(g => g.IsActive).ToList();
        return games;
    }

    public async Task<IReadOnlyList<Game>> GetAllActivesGamesAsync(CancellationToken ct = default)
    {
        const string sql = """
        SELECT *
        FROM Game
        WHERE IsActive = 1
        """;

        using IDbConnection connection = _connectionFactory.CreateConnection();
        long startTimestamp = Stopwatch.GetTimestamp();

        IEnumerable<Game> games = await connection.QueryAsync<Game>(
            new CommandDefinition(sql, cancellationToken: ct));

        List<Game> activeGames = games.ToList();
        LogDapperCommand(nameof(GetAllActivesGamesAsync), sql, activeGames.Count, startTimestamp);

        return activeGames;
    }

    public async Task<IReadOnlyList<GameListRow>> GetActiveGamesWithBestDiscountAsync(CancellationToken ct = default)
    {
        const string sql = """
        SELECT 
            g.Id, 
            g.Title, 
            g.Description,
            g.Price AS OriginalPrice, 
            -- ROUND não reduz a escala da expressão (vira decimal(x,9)); o CAST alinha ao tipo de Game.Price.
            CAST(
                ROUND(
                    g.Price * (100.0 - ISNULL(p.DiscountPercentage, 0.0)) / 100.0,
                    2
                ) AS DECIMAL(18,2)
            ) AS FinalPrice,
            g.ReleaseDate
        FROM Game g
        OUTER APPLY (
            SELECT MAX(pr.DiscountPercentage) AS DiscountPercentage
            FROM Promotion pr
            WHERE pr.GameId = g.Id
              AND pr.IsActive = 1
              AND pr.StartDate <= @Now
              AND pr.EndDate >= @Now
        ) p
        WHERE g.IsActive = 1
        ORDER BY g.Title;
        """;

        using IDbConnection connection = _connectionFactory.CreateConnection();
        long startTimestamp = Stopwatch.GetTimestamp();

        var rows = await connection.QueryAsync<GameListRow>(
            new CommandDefinition(
                sql,
                new { Now = DateTime.UtcNow },
                cancellationToken: ct));

        List<GameListRow> gameRows = rows.AsList(); // AsList() evita re-alocação se já for um List<T> retornado pelo Dapper
        LogDapperCommand(nameof(GetActiveGamesWithBestDiscountAsync), sql, gameRows.Count, startTimestamp);

        return gameRows;
    }

    public Game? GetGameById(Guid id)
    {
        return _context.Game.FirstOrDefault(g => g.Id == id) ?? null;
    }

    public void UpdateGame(Guid Id, string? title, string? description, decimal? price, DateTime? releaseDate)
    {
        const string sql = @"
        UPDATE Game
        SET Title = COALESCE(@Title, Title),
            Description = COALESCE(@Description, Description),
            Price = COALESCE(@Price, Price),
            ReleaseDate = COALESCE(@ReleaseDate, ReleaseDate)
        WHERE Id = @Id;";

        using IDbConnection connection = _connectionFactory.CreateConnection();
        long startTimestamp = Stopwatch.GetTimestamp();

        int rowsAffected = connection.Execute(sql, new
        {
            Id,
            Title = title,
            Description = description,
            Price = price,
            ReleaseDate = releaseDate
        });

        LogDapperCommand(nameof(UpdateGame), sql, rowsAffected, startTimestamp);

        if (rowsAffected == 0)
            throw new KeyNotFoundException($"Game {Id} not found.");
    }

    public bool VerifyIfExist(string name, DateTime releaseDate) =>  _context.Game.Any(g => g.Title == name && g.ReleaseDate == releaseDate);
    public bool VerifyIfExist(Guid id) => _context.Game.Any(g => g.Id == id);

    private void LogDapperCommand(string operation, string sql, int rowCount, long startTimestamp)
    {
        _logger.LogInformation("Dapper executou {Operation} em {ElapsedMilliseconds} ms ({RowCount} linhas). SQL: {Sql}",
            operation,
            Math.Round(Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds, 2),
            rowCount,
            sql);
    }
}
