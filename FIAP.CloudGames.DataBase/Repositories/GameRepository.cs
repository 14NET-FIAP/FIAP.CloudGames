using Dapper;
using FIAP.CloudGames.DataBase.Context;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FIAP.CloudGames.DataBase.Repositories;

public class GameRepository : IGameRepository
{
    private readonly AppDbContext _context;
    private readonly ISqlServerConnectionFactory _connectionFactory;
    public GameRepository(AppDbContext context, ISqlServerConnectionFactory sqlServerConnectionFactory)
    {
        _context = context;
        _connectionFactory = sqlServerConnectionFactory;
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

        IEnumerable<Game> games = await connection.QueryAsync<Game>(
            new CommandDefinition(sql, cancellationToken: ct));

        return games.ToList();
    }

    public async Task<IReadOnlyList<GameListRow>> GetActiveGamesWithBestDiscountAsync(CancellationToken ct = default)
    {
        const string sql = """
        SELECT 
            g.Id, 
            g.Title, 
            g.Description,
            g.Price AS OriginalPrice, 
            ROUND(
                g.Price * (100.0 - ISNULL(p.DiscountPercentage, 0.0)) / 100.0, 
                2
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

        var rows = await connection.QueryAsync<GameListRow>(
            new CommandDefinition(
                sql,
                new { Now = DateTime.UtcNow },
                cancellationToken: ct));

        return rows.AsList(); // AsList() evita re-alocação se já for um List<T> retornado pelo Dapper
    }

    public Game? GetGameById(Guid id)
    {
        return _context.Game.FirstOrDefault(g => g.Id == id) ?? null;
    }

    public void UpdateGame(Guid Id, Game newGame)
    {
        const string sql = @"
        UPDATE Game
        SET Title = COALESCE(@Title, Title),
            Description = COALESCE(@Description, Description),
            Price = @Price,
            ReleaseDate = COALESCE(@ReleaseDate, ReleaseDate)
        WHERE Id = @Id;";

        using IDbConnection connection = _connectionFactory.CreateConnection();

        int rowsAffected = connection.Execute(sql, new
        {
            Id,
            newGame.Title,
            newGame.Description,
            newGame.Price,
            newGame.ReleaseDate
        });

        if (rowsAffected == 0)
            throw new KeyNotFoundException($"Game {Id} not found.");
    }

    public bool VerifyIfExist(string name, DateTime releaseDate) =>  _context.Game.Any(g => g.Title == name && g.ReleaseDate == releaseDate);
    public bool VerifyIfExist(Guid id) => _context.Game.Any(g => g.Id == id);
}
