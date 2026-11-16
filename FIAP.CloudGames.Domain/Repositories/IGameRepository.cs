using FIAP.CloudGames.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Domain.Repositories;

public interface IGameRepository
{
    void CreateGame(Game game);
    IEnumerable<Game> GetAllActivesGames();
    Task<IReadOnlyList<Game>> GetAllActivesGamesAsync(CancellationToken ct = default);
    Game? GetGameById(Guid id);
    void UpdateGame(Guid id, Game gameDto);
    bool VerifyIfExist(string name, DateTime releaseDate);
    bool VerifyIfExist(Guid id);
}
