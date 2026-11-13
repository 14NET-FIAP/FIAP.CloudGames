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

    public Result CreateNewGame(CreateGameDto gameDto)
    {
        try
        {
            Game game = GameMapping.MapGameToCreate(gameDto);

            if (VerifyGameExists(game.Title, game.ReleaseDate.Value))
                return Result.Failure("Game already exists.");

            _gameRepository.CreateGame(game);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while creating the game: {ex.Message}");
        }

    }

    public ResultList<GamesListDto> GetAllGames()
    {
        IEnumerable<Game> games = _gameRepository.GetAllActivesGames();

        DateTime currentDate = DateTime.UtcNow;
        ILookup<Guid, Promotion> promotionsByGame = _promotionRepository
            .GetActivePromotions(currentDate)
            .ToLookup(promotion => promotion.GameId);

        List<GamesListDto> gameDtos = games.Select(game =>
        {
            Promotion? promotion = promotionsByGame[game.Id]
                .OrderByDescending(d => d.DiscountPercentage)
                .FirstOrDefault();

            decimal priceWithDiscount = promotion is null
                ? game.Price.Value
                : game.Price.Value * (100m - promotion.DiscountPercentage) / 100m;

            return game.MapGameToListDto(priceWithDiscount);
        }).ToList();

        return ResultList<GamesListDto>.Success(gameDtos);
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
}
