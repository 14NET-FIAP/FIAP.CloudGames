using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.DataBase.Context;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.DataBase.Repositories;

public class GameRepository : IGameRepository
{
    private readonly AppDbContext _context;
    public GameRepository(AppDbContext context)
    {
        _context = context;
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
        game.Price = newGame.Price ?? game.Price;
        game.ReleaseDate = newGame.ReleaseDate ?? game.ReleaseDate;

        _context.SaveChanges();
    }

    public bool VerifyIfExist(string name, DateTime releaseDate) =>  _context.Game.Any(g => g.Title == name && g.ReleaseDate == releaseDate);
    public bool VerifyIfExist(Guid id) => _context.Game.Any(g => g.Id == id);
}
