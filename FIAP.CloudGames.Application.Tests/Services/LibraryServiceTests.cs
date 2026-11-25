using FIAP.CloudGames.Application.DTOs.Library;
using FIAP.CloudGames.Application.Services;
using FIAP.CloudGames.Application.Tests.Fakes;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Exceptions;
using FIAP.CloudGames.Domain.ReadModels;
using Microsoft.Extensions.Logging.Abstractions;

namespace FIAP.CloudGames.Application.Tests.Services;

public class LibraryServiceTests
{
    private const string TechnicalErrorDetail = "Timeout expired on SqlConnection";

    private const string UserNotFoundMessage = "Usuário não encontrado.";
    private const string GameUnavailableMessage = "Jogo não encontrado ou indisponível.";
    private const string GameAlreadyInLibraryMessage = "O jogo já está na biblioteca do usuário.";

    private readonly FakeLibraryRepository _libraryRepository = new();
    private readonly FakeUserRepository _userRepository = new();
    private readonly FakeGameRepository _gameRepository = new();
    private readonly FakePromotionRepository _promotionRepository = new();
    private readonly LibraryService _service;

    public LibraryServiceTests()
    {
        _service = new LibraryService(
            _libraryRepository,
            _userRepository,
            _gameRepository,
            _promotionRepository,
            NullLogger<LibraryService>.Instance);
    }

    [Fact]
    public async Task GetLibraryByUserId_WithValidId_ReturnsMappedGames()
    {
        Guid userId = Guid.NewGuid();
        var row = new UserLibraryItemRow(Guid.NewGuid(), "Celeste", "Plataforma", 36.99m, new DateTime(2018, 1, 25), new DateTime(2025, 3, 1));
        _libraryRepository.Rows = [row];

        var result = await _service.GetLibraryByUserId(userId);

        Assert.True(result.IsSuccess);
        var dto = Assert.Single(result.Value);
        Assert.Equal(row.GameId, dto.GameId);
        Assert.Equal(row.Title, dto.Title);
        Assert.Equal(row.Description, dto.Description);
        Assert.Equal(row.PricePaid, dto.PricePaid);
        Assert.Equal(row.ReleaseDate, dto.ReleaseDate);
        Assert.Equal(row.AcquisitionDate, dto.AcquisitionDate);
        Assert.Equal(userId, _libraryRepository.RequestedUserId);
    }

    [Fact]
    public async Task GetLibraryByUserId_WhenLibraryIsEmpty_ReturnsEmptySuccess()
    {
        var result = await _service.GetLibraryByUserId(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetLibraryByUserId_ForwardsCancellationToken()
    {
        using var cancellationSource = new CancellationTokenSource();

        await _service.GetLibraryByUserId(Guid.NewGuid(), cancellationSource.Token);

        Assert.Equal(cancellationSource.Token, _libraryRepository.RequestedToken);
    }

    [Fact]
    public async Task GetLibraryByUserId_WhenRepositoryThrows_PropagatesException()
    {
        _libraryRepository.ExceptionToThrow = new InvalidOperationException(TechnicalErrorDetail);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.GetLibraryByUserId(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetLibraryByUserId_WhenOperationIsCanceled_PropagatesCancellation()
    {
        _libraryRepository.ExceptionToThrow = new OperationCanceledException();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _service.GetLibraryByUserId(Guid.NewGuid()));
    }

    [Fact]
    public async Task AddGameToLibraryAsync_WithoutPromotion_StoresFullPrice()
    {
        User user = CreateActiveUser();
        Game game = CreateActiveGame(price: 100m);

        var result = await _service.AddGameToLibraryAsync(user.Id, new AddGameToLibraryDto(game.Id));

        Assert.True(result.IsSuccess);
        UserLibrary added = Assert.IsType<UserLibrary>(_libraryRepository.AddedItem);
        Assert.Equal(user.Id, added.UserId);
        Assert.Equal(game.Id, added.GameId);
        Assert.Equal(100m, added.PricePaid);
    }

    [Fact]
    public async Task AddGameToLibraryAsync_WithActivePromotions_StoresPriceWithBestDiscount()
    {
        User user = CreateActiveUser();
        Game game = CreateActiveGame(price: 59.90m);
        _promotionRepository.ActivePromotions =
        [
            new Promotion(10m, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), game.Id),
            new Promotion(25m, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), game.Id)
        ];

        var result = await _service.AddGameToLibraryAsync(user.Id, new AddGameToLibraryDto(game.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(44.93m, _libraryRepository.AddedItem!.PricePaid);
    }

    [Fact]
    public async Task AddGameToLibraryAsync_ForwardsCancellationToken()
    {
        User user = CreateActiveUser();
        Game game = CreateActiveGame(price: 100m);
        using var cancellationSource = new CancellationTokenSource();

        await _service.AddGameToLibraryAsync(user.Id, new AddGameToLibraryDto(game.Id), cancellationSource.Token);

        Assert.Equal(cancellationSource.Token, _promotionRepository.RequestedPromotionsToken);
    }

    [Fact]
    public async Task AddGameToLibraryAsync_WhenUserDoesNotExistOrIsInactive_ReturnsNotFound()
    {
        Game game = CreateActiveGame(price: 100m);

        var result = await _service.AddGameToLibraryAsync(Guid.NewGuid(), new AddGameToLibraryDto(game.Id));

        Assert.False(result.IsSuccess);
        Assert.Equal(UserNotFoundMessage, result.Message);
        Assert.Null(_libraryRepository.AddedItem);
    }

    [Fact]
    public async Task AddGameToLibraryAsync_WhenGameDoesNotExist_ReturnsGameUnavailable()
    {
        User user = CreateActiveUser();

        var result = await _service.AddGameToLibraryAsync(user.Id, new AddGameToLibraryDto(Guid.NewGuid()));

        Assert.False(result.IsSuccess);
        Assert.Equal(GameUnavailableMessage, result.Message);
        Assert.Null(_libraryRepository.AddedItem);
    }

    [Fact]
    public async Task AddGameToLibraryAsync_WhenGameIsInactive_ReturnsGameUnavailable()
    {
        User user = CreateActiveUser();
        Game game = CreateActiveGame(price: 100m);
        game.IsActive = false;

        var result = await _service.AddGameToLibraryAsync(user.Id, new AddGameToLibraryDto(game.Id));

        Assert.False(result.IsSuccess);
        Assert.Equal(GameUnavailableMessage, result.Message);
        Assert.Null(_libraryRepository.AddedItem);
    }

    [Fact]
    public async Task AddGameToLibraryAsync_WhenGameIsAlreadyInLibrary_ReturnsFailureWithoutAdding()
    {
        User user = CreateActiveUser();
        Game game = CreateActiveGame(price: 100m);
        _libraryRepository.GameAlreadyInLibrary = true;

        var result = await _service.AddGameToLibraryAsync(user.Id, new AddGameToLibraryDto(game.Id));

        Assert.False(result.IsSuccess);
        Assert.Equal(GameAlreadyInLibraryMessage, result.Message);
        Assert.Null(_libraryRepository.AddedItem);
    }

    [Fact]
    public async Task AddGameToLibraryAsync_WhenConcurrentInsertViolatesPrimaryKey_ReturnsGameAlreadyInLibrary()
    {
        User user = CreateActiveUser();
        Game game = CreateActiveGame(price: 100m);
        _libraryRepository.ExceptionToThrowOnWrite =
            new DuplicateEntityException(GameAlreadyInLibraryMessage, new Exception("2627"));

        var result = await _service.AddGameToLibraryAsync(user.Id, new AddGameToLibraryDto(game.Id));

        Assert.False(result.IsSuccess);
        Assert.Equal(GameAlreadyInLibraryMessage, result.Message);
    }

    [Fact]
    public async Task AddGameToLibraryAsync_WhenUnexpectedErrorOccurs_PropagatesException()
    {
        User user = CreateActiveUser();
        Game game = CreateActiveGame(price: 100m);
        _libraryRepository.ExceptionToThrowOnWrite = new InvalidOperationException(TechnicalErrorDetail);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AddGameToLibraryAsync(user.Id, new AddGameToLibraryDto(game.Id)));
    }

    [Fact]
    public async Task AddGameToLibraryAsync_WhenOperationIsCanceled_PropagatesCancellation()
    {
        User user = CreateActiveUser();
        Game game = CreateActiveGame(price: 100m);
        _libraryRepository.ExceptionToThrowOnWrite = new OperationCanceledException();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _service.AddGameToLibraryAsync(user.Id, new AddGameToLibraryDto(game.Id)));
    }

    [Fact]
    public void RemoveGameFromLibrary_WhenGameIsInLibrary_RemovesIt()
    {
        Guid userId = Guid.NewGuid();
        Guid gameId = Guid.NewGuid();

        var result = _service.RemoveGameFromLibrary(userId, gameId);

        Assert.True(result.IsSuccess);
        Assert.Equal((userId, gameId), _libraryRepository.RemovedItem);
    }

    [Fact]
    public void RemoveGameFromLibrary_WhenGameIsNotInLibrary_ReturnsFailure()
    {
        _libraryRepository.GameWasInLibrary = false;

        var result = _service.RemoveGameFromLibrary(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("O jogo não está na biblioteca do usuário.", result.Message);
    }

    [Fact]
    public void RemoveGameFromLibrary_WhenUnexpectedErrorOccurs_PropagatesException()
    {
        _libraryRepository.ExceptionToThrowOnWrite = new InvalidOperationException(TechnicalErrorDetail);

        Assert.Throws<InvalidOperationException>(() => _service.RemoveGameFromLibrary(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public void RemoveGameFromLibrary_WhenOperationIsCanceled_PropagatesCancellation()
    {
        _libraryRepository.ExceptionToThrowOnWrite = new OperationCanceledException();

        Assert.Throws<OperationCanceledException>(() => _service.RemoveGameFromLibrary(Guid.NewGuid(), Guid.NewGuid()));
    }

    private User CreateActiveUser()
    {
        var user = new User("Maria", "maria@email.com", "hash");
        _userRepository.ActiveUser = user;
        return user;
    }

    private Game CreateActiveGame(decimal price)
    {
        var game = new Game("Celeste", "Plataforma", price, new DateTime(2018, 1, 25));
        _gameRepository.GameById = game;
        return game;
    }
}
