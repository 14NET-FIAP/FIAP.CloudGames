using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Exceptions;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Domain.Services;
using FIAP.CloudGames.Application.DTOs.Library;
using Microsoft.Extensions.Logging;

namespace FIAP.CloudGames.Application.Services;

public class LibraryService : ILibraryService
{
    private const decimal NoDiscount = 0m;

    private const string InvalidUserIdMessage = "Identificador de usuário inválido.";
    private const string UnexpectedErrorMessage = "Não foi possível carregar a biblioteca de jogos.";
    private const string UserNotFoundMessage = "Usuário não encontrado.";
    private const string GameUnavailableMessage = "Jogo não encontrado ou indisponível.";
    private const string GameAlreadyInLibraryMessage = "O jogo já está na biblioteca do usuário.";
    private const string GameNotInLibraryMessage = "O jogo não está na biblioteca do usuário.";
    private const string AddGameErrorMessage = "Não foi possível adicionar o jogo à biblioteca.";
    private const string RemoveGameErrorMessage = "Não foi possível remover o jogo da biblioteca.";

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

    public async Task<ResultList<LibraryListGamesDto>> GetLibraryByUserId(string UserId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(UserId, out Guid userId))
        {
            _logger.LogWarning("Busca de biblioteca recusada: identificador de usuário inválido {UserId}", UserId);
            return ResultList<LibraryListGamesDto>.Failure(InvalidUserIdMessage);
        }

        try
        {
            var result = await _libraryRepository.GetUserLibraryAsync(userId, ct);

            var dtos = new List<LibraryListGamesDto>(result.Count);
            foreach (var userLibraryItem in result)
                dtos.Add(userLibraryItem.MapLibraryRowToDto());

            return ResultList<LibraryListGamesDto>.Success(dtos);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Erro ao buscar a biblioteca do usuário {UserId}", userId);
            return ResultList<LibraryListGamesDto>.Failure(UnexpectedErrorMessage);
        }
    }

    public async Task<Result> AddGameToLibraryAsync(string UserId, AddGameToLibraryDto libraryDto, CancellationToken ct = default)
    {
        if (!Guid.TryParse(UserId, out Guid userId))
        {
            _logger.LogWarning("Inclusão na biblioteca recusada: identificador de usuário inválido {UserId}", UserId);
            return Result.Failure(InvalidUserIdMessage);
        }

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

            decimal pricePaid = game.CalculatePriceWithDiscount(await GetBestActiveDiscountAsync(gameId, ct));
            _libraryRepository.AddGameToLibrary(new UserLibrary(userId, gameId, pricePaid));

            _logger.LogInformation("Jogo {GameId} adicionado à biblioteca do usuário {UserId} por {PricePaid}", gameId, userId, pricePaid);
            return Result.Success();
        }
        catch (DuplicateEntityException)
        {
            // Outra requisição incluiu o mesmo jogo entre a verificação e o insert.
            _logger.LogWarning("Inclusão do jogo {GameId} na biblioteca do usuário {UserId} rejeitada pela chave primária (inclusão concorrente)", gameId, userId);
            return Result.Failure(GameAlreadyInLibraryMessage);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Erro ao adicionar o jogo {GameId} à biblioteca do usuário {UserId}", gameId, userId);
            return Result.Failure(AddGameErrorMessage);
        }
    }

    public Result RemoveGameFromLibrary(Guid userId, Guid gameId)
    {
        try
        {
            if (!_libraryRepository.RemoveGameFromLibrary(userId, gameId))
                return Result.Failure(GameNotInLibraryMessage);

            _logger.LogInformation("Jogo {GameId} removido da biblioteca do usuário {UserId}", gameId, userId);
            return Result.Success();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Erro ao remover o jogo {GameId} da biblioteca do usuário {UserId}", gameId, userId);
            return Result.Failure(RemoveGameErrorMessage);
        }
    }

    private async Task<decimal> GetBestActiveDiscountAsync(Guid gameId, CancellationToken ct)
    {
        IReadOnlyList<Promotion> promotions = await _promotionRepository.GetActivePromotionsByGameIdsAsync([gameId], ct);

        return promotions.Count == 0 ? NoDiscount : promotions.Max(p => p.DiscountPercentage);
    }
}
