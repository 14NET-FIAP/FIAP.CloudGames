using FIAP.CloudGames.Application.DTOs.Library;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Exceptions;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Domain.Services;
using Microsoft.Extensions.Logging;

namespace FIAP.CloudGames.Application.Services;

public class LibraryService : ILibraryService
{
    private const decimal NoDiscount = 0m;

    private const string UserNotFoundMessage = "Usuário não encontrado.";
    private const string GameUnavailableMessage = "Jogo não encontrado ou indisponível.";
    private const string GameAlreadyInLibraryMessage = "O jogo já está na biblioteca do usuário.";
    private const string GameNotInLibraryMessage = "O jogo não está na biblioteca do usuário.";

    private readonly ILibraryRepository _libraryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IPromotionRepository _promotionRepository;
    private readonly ILogger<LibraryService> _logger;

    public LibraryService(
        ILibraryRepository libraryRepository,
        IUserRepository userRepository,
        IGameRepository gameRepository,
        IPromotionRepository promotionRepository,
        ILogger<LibraryService> logger)
    {
        _libraryRepository = libraryRepository;
        _userRepository = userRepository;
        _gameRepository = gameRepository;
        _promotionRepository = promotionRepository;
        _logger = logger;
    }

    public async Task<ResultList<LibraryListGamesDto>> GetLibraryByUserId(Guid userId, CancellationToken ct = default)
    {
        var libraryItems = await _libraryRepository.GetUserLibraryAsync(userId, ct);

        var dtos = new List<LibraryListGamesDto>(libraryItems.Count);
        foreach (var userLibraryItem in libraryItems)
            dtos.Add(userLibraryItem.MapLibraryRowToDto());

        return ResultList<LibraryListGamesDto>.Success(dtos);
    }

    public async Task<Result> AddGameToLibraryAsync(Guid userId, AddGameToLibraryDto libraryDto, CancellationToken ct = default)
    {
        Guid gameId = libraryDto.GameId;

        try
        {
            if (_userRepository.GetActiveUserById(userId) is null)
                return Result.Failure(UserNotFoundMessage);

            Game? game = _gameRepository.GetGameById(gameId);
            if (game is null || !game.IsActive)
                return Result.Failure(GameUnavailableMessage);

            if (_libraryRepository.VerifyIfExist(userId, gameId))
                return Result.Failure(GameAlreadyInLibraryMessage);

            decimal pricePaid = libraryDto.PricePaid ?? await CalculateCurrentPriceAsync(game, ct);
            _libraryRepository.AddGameToLibrary(new UserLibrary(userId, gameId, pricePaid));

            _logger.LogInformation("Jogo {GameId} adicionado à biblioteca do usuário {UserId} por {PricePaid}", gameId, userId, pricePaid);
            return Result.Success();
        }
        catch (DuplicateEntityException)
        {
            _logger.LogWarning("Inclusão do jogo {GameId} na biblioteca do usuário {UserId} rejeitada pela chave primária (inclusão concorrente)", gameId, userId);
            return Result.Failure(GameAlreadyInLibraryMessage);
        }
    }

    public Result RemoveGameFromLibrary(Guid userId, Guid gameId)
    {
        if (!_libraryRepository.RemoveGameFromLibrary(userId, gameId))
            return Result.Failure(GameNotInLibraryMessage);

        _logger.LogInformation("Jogo {GameId} removido da biblioteca do usuário {UserId}", gameId, userId);
        return Result.Success();
    }

    private async Task<decimal> CalculateCurrentPriceAsync(Game game, CancellationToken ct)
    {
        decimal pricePaid = game.CalculatePriceWithDiscount(await GetBestActiveDiscountAsync(game.Id, ct));

        _logger.LogDebug("Preço pago não informado para o jogo {GameId}; calculado com o maior desconto ativo: {PricePaid}", game.Id, pricePaid);
        return pricePaid;
    }

    private async Task<decimal> GetBestActiveDiscountAsync(Guid gameId, CancellationToken ct)
    {
        IReadOnlyList<Promotion> promotions = await _promotionRepository.GetActivePromotionsByGameIdsAsync([gameId], ct);
        return promotions.Count == 0 ? NoDiscount : promotions.Max(p => p.DiscountPercentage);
    }
}
