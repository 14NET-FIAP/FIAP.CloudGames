using Dapper;
using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.DataBase.Context;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public async Task<IReadOnlyList<GameListRow>> GetActiveGamesWithBestDiscountAsync(
    DateTime now, int page, int pageSize, CancellationToken ct = default)
    {
        const string sql = """
        SELECT g.Id, g.Title, g.Price, g.ReleaseDate, g.Description,
               p.DiscountPercentage
        FROM Game g
        OUTER APPLY (
            SELECT TOP (1) pr.DiscountPercentage
            FROM Promotion pr
            WHERE pr.GameId = g.Id
              AND pr.IsActive = 1
              AND pr.StartDate <= @Now
              AND pr.EndDate >= @Now
            ORDER BY pr.DiscountPercentage DESC
        ) p
        WHERE g.IsActive = 1
        ORDER BY g.Title
        --OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
        """;

        using IDbConnection connection = _connectionFactory.CreateConnection();

        IEnumerable<GameListRow> rows = await connection.QueryAsync<GameListRow>(
            new CommandDefinition(
                sql,
                new { Now = now, Offset = (page - 1) * pageSize, PageSize = pageSize },
                cancellationToken: ct));

        return rows.ToList();
    }

    public Game? GetGameById(Guid id)
    {
        return _context.Game.FirstOrDefault(g => g.Id == id) ?? null;
    }

    public void UpdateGame(Guid id, Game newGame)
    {
        var game = _context.Game.FirstOrDefault(g => g.Id == id);

        if (game == null)
            throw new KeyNotFoundException($"Game {id} not found.");

        game.Title = newGame.Title ?? game.Title;
        game.Description = newGame.Description ?? game.Description;
        game.Price = newGame.Price;
        game.ReleaseDate = newGame.ReleaseDate ?? game.ReleaseDate;

        _context.SaveChanges();
    }

    public bool VerifyIfExist(string name, DateTime releaseDate) =>  _context.Game.Any(g => g.Title == name && g.ReleaseDate == releaseDate);
    public bool VerifyIfExist(Guid id) => _context.Game.Any(g => g.Id == id);
}
