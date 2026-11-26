using FIAP.CloudGames.Application.DTOs.Games;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Exceptions;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Domain.Services;
using Microsoft.Extensions.Logging;

namespace FIAP.CloudGames.Application.Services;

public class GamesService : IGamesService
{
    private const string GameAlreadyExistsMessage = "Jogo já cadastrado.";
    private const string GameNotFoundMessage = "Jogo não encontrado.";

    private readonly IGameRepository _gameRepository;
    private readonly IPromotionRepository _promotionRepository;
    private readonly ILogger<GamesService> _logger;

    public GamesService(IGameRepository gameRepository, IPromotionRepository promotionRepository, ILogger<GamesService> logger)
    {
        _gameRepository = gameRepository;
        _promotionRepository = promotionRepository;
        _logger = logger;
    }

    public Result<Guid> CreateNewGame(CreateGameDto gameDto)
    {
        try
        {
            Game game = GameMapping.MapGameToCreate(gameDto);

            if (VerifyGameExists(game.Title, game.ReleaseDate!.Value))
                return Result<Guid>.Failure(GameAlreadyExistsMessage);

            _gameRepository.CreateGame(game);
            return Result<Guid>.Success(game.Id);
        }
        catch (DomainException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
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
        try
        {
            Game? game = _gameRepository.GetGameById(id);
            if (game is null)
                return Result.Failure(GameNotFoundMessage);

            game.Update(gameDto.title, gameDto.description, gameDto.price, gameDto.releaseDate);
            _gameRepository.UpdateGame(game);

            _logger.LogInformation("Jogo {GameId} atualizado", id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public Result DeleteGame(Guid id)
    {
        Game? game = _gameRepository.GetGameById(id);
        if (game is null)
            return Result.Failure(GameNotFoundMessage);

        game.Deactivate();
        _gameRepository.UpdateGame(game);

        _logger.LogInformation("Jogo {GameId} desativado", id);
        return Result.Success();
    }

    private bool VerifyGameExists(string name, DateTime releaseDate) => _gameRepository.VerifyIfExist(name, releaseDate);
}
