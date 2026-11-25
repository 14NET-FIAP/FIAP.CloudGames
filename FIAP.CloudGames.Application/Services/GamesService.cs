using FIAP.CloudGames.Application.DTOs.Games;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Domain.Services;

namespace FIAP.CloudGames.Application.Services;

public class GamesService : IGamesService
{
    private readonly IGameRepository _gameRepository;
    private readonly IPromotionRepository _promotionRepository;

    public GamesService(IGameRepository gameRepository, IPromotionRepository promotionRepository)
    {
        _gameRepository = gameRepository;
        _promotionRepository = promotionRepository;
    }

    public Result<Guid> CreateNewGame(CreateGameDto gameDto)
    {
        Game game = GameMapping.MapGameToCreate(gameDto);

        if (VerifyGameExists(game.Title, game.ReleaseDate!.Value))
            return Result<Guid>.Failure("Game already exists.");

        _gameRepository.CreateGame(game);
        return Result<Guid>.Success(game.Id);
    }

    public async Task<ResultList<GamesListDto>> GetAllActivesGamesAsync(CancellationToken ct = default)
    {
        IReadOnlyList<GameListRow> games = await _gameRepository.GetActiveGamesWithBestDiscountAsync(ct);

        if (games.Count == 0) return ResultList<GamesListDto>.Success();

        var dtos = new List<GamesListDto>(games.Count);
        foreach (var game in games)
            dtos.Add(game.MapGameToListDto());

        return ResultList<GamesListDto>.Success(dtos);
    }

    public Result UpdateGame(UpdateGameDto gameDto, Guid id)
    {
        if (!VerifyGameExists(id)) return Result.Failure("Game not found.");

        _gameRepository.UpdateGame(id, gameDto.title, gameDto.description, gameDto.price, gameDto.releaseDate);

        return Result.Success();
    }

    private bool VerifyGameExists(string name, DateTime releaseDate) => _gameRepository.VerifyIfExist(name, releaseDate);
    private bool VerifyGameExists(Guid id) => _gameRepository.VerifyIfExist(id);

    //public ResultList<GamesListDto> GetAllGames()
    //{
    //    throw new NotImplementedException();
    //}
}
