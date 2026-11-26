using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
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
    Task<IReadOnlyList<GameListRow>> GetActiveGamesWithBestDiscountAsync(/*int page, int pageSize, */CancellationToken ct = default);
    Game? GetGameById(Guid id);
    void UpdateGame(Game game);
    bool VerifyIfExist(string name, DateTime releaseDate);
    bool VerifyIfExist(Guid id);
}
