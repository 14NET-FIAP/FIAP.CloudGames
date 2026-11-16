using FIAP.CloudGames.Application.DTOs.Games;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.Domain.Entities;
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
        try
        {
            Game game = GameMapping.MapGameToCreate(gameDto);

            if (VerifyGameExists(game.Title, game.ReleaseDate!.Value))
                return Result<Guid>.Failure("Game already exists.");

            _gameRepository.CreateGame(game);
            return Result<Guid>.Success(new Guid());
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"An error occurred while creating the game: {ex.Message}");
        }

    }

    public async Task<ResultList<GamesListDto>> GetAllGamesAsync(CancellationToken ct = default)
    {
        try
        {
            IReadOnlyList<Game> games = await _gameRepository.GetAllActivesGamesAsync(ct);

            if (games.Count == 0)
                return ResultList<GamesListDto>.Success(new List<GamesListDto>());

            DateTime currentDate = DateTime.UtcNow;

            IReadOnlyList<Promotion> promotions = await _promotionRepository
                .GetActivePromotionsByGameIdsAsync(games.Select(g => g.Id), currentDate, ct);

            // Já guarda só a MAIOR promoção de cada jogo
            Dictionary<Guid, Promotion> bestPromotionByGame = promotions
                .GroupBy(p => p.GameId)
                .ToDictionary(
                    g => g.Key,
                    g => g.MaxBy(p => p.DiscountPercentage)!);

            List<GamesListDto> gameDtos = games
                .Select(game =>
                {
                    decimal price = game.Price!.Value;

                    if (bestPromotionByGame.TryGetValue(game.Id, out Promotion? promotion))
                    {
                        price = CalculateDiscountedPrice(price, promotion.DiscountPercentage);
                    }

                    return game.MapGameToDatailDto(price);
                })
                .ToList();

            return ResultList<GamesListDto>.Success(gameDtos);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ResultList<GamesListDto>.Failure($"Não foi possível carregar os jogos: {ex.Message}");
        }
    }

    private static decimal CalculateDiscountedPrice(decimal price, decimal discountPercentage)
    {
        decimal safeDiscount = Math.Clamp(discountPercentage, 0m, 100m);
        return Math.Round(price * (100m - safeDiscount) / 100m, 2, MidpointRounding.AwayFromZero);
    }

    public Result UpdateGame(UpdateGameDto gameDto, Guid id)
    {
        try
        {
            if (!VerifyGameExists(id)) return Result.Failure("Game not found.");

            var gameMapping = GameMapping.MapGameToUpdate(gameDto, id);
            _gameRepository.UpdateGame(id, gameMapping);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while updating the game: {ex.Message}");
        }
    }

    private bool VerifyGameExists(string name, DateTime releaseDate) => _gameRepository.VerifyIfExist(name, releaseDate);
    private bool VerifyGameExists(Guid id) => _gameRepository.VerifyIfExist(id);

    public ResultList<GamesListDto> GetAllGames()
    {
        throw new NotImplementedException();
    }
}
