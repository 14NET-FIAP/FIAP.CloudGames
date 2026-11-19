using FIAP.CloudGames.Application.DTOs.Games;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Services;

namespace FIAP.CloudGames.Application.Interfaces;

public interface IGamesService
{
    Result<Guid> CreateNewGame(CreateGameDto gameDto);
    ResultList<GamesListDto> GetAllGames();
    Result UpdateGame(UpdateGameDto gameDto, Guid id);
    Task<ResultList<GamesListDto>> GetAllActivesGamesAsync(CancellationToken ct = default);
}
