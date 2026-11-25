using FIAP.CloudGames.Application.DTOs.Games;
using FIAP.CloudGames.Application.Services;
using FIAP.CloudGames.Application.Tests.Fakes;

namespace FIAP.CloudGames.Application.Tests.Services;

public class GamesServiceTests
{
    private static readonly DateTime ReleaseDate = new(2024, 5, 10);

    private readonly FakeGameRepository _gameRepository = new();
    private readonly GamesService _service;

    public GamesServiceTests()
    {
        _service = new GamesService(_gameRepository, new FakePromotionRepository());
    }

    [Fact]
    public void CreateNewGame_WhenGameIsNew_ReturnsIdOfCreatedGame()
    {
        var result = _service.CreateNewGame(new CreateGameDto("Hollow Knight", "Metroidvania", 49.90m, ReleaseDate));

        Assert.True(result.IsSuccess);
        Assert.NotNull(_gameRepository.CreatedGame);
        Assert.NotEqual(Guid.Empty, result.Value);
        Assert.Equal(_gameRepository.CreatedGame!.Id, result.Value);
    }

    [Fact]
    public void CreateNewGame_WhenGameAlreadyExists_ReturnsFailureWithoutCreating()
    {
        _gameRepository.GameExistsByTitleAndDate = true;

        var result = _service.CreateNewGame(new CreateGameDto("Hollow Knight", "Metroidvania", 49.90m, ReleaseDate));

        Assert.False(result.IsSuccess);
        Assert.Equal(Guid.Empty, result.Value);
        Assert.Null(_gameRepository.CreatedGame);
    }

    [Fact]
    public void CreateNewGame_WhenRepositoryThrows_PropagatesException()
    {
        _gameRepository.ExceptionToThrow = new InvalidOperationException("falha no banco");

        Assert.Throws<InvalidOperationException>(
            () => _service.CreateNewGame(new CreateGameDto("Hollow Knight", "Metroidvania", 49.90m, ReleaseDate)));
    }

    [Fact]
    public void UpdateGame_WithOnlyTitle_ForwardsNullForOmittedFields()
    {
        Guid gameId = Guid.NewGuid();

        var result = _service.UpdateGame(new UpdateGameDto(null, "Novo título", null, null, null), gameId);

        Assert.True(result.IsSuccess);
        Assert.Equal(new GameUpdateCall(gameId, "Novo título", null, null, null), _gameRepository.UpdateCall);
    }

    [Fact]
    public void UpdateGame_WithAllFieldsNull_SucceedsWithoutChangingValues()
    {
        Guid gameId = Guid.NewGuid();

        var result = _service.UpdateGame(new UpdateGameDto(null, null, null, null, null), gameId);

        Assert.True(result.IsSuccess);
        Assert.Equal(new GameUpdateCall(gameId, null, null, null, null), _gameRepository.UpdateCall);
    }

    [Fact]
    public void UpdateGame_WithAllFields_ForwardsAllValues()
    {
        Guid gameId = Guid.NewGuid();

        var result = _service.UpdateGame(new UpdateGameDto(null, "Título", "Descrição", 19.90m, ReleaseDate), gameId);

        Assert.True(result.IsSuccess);
        Assert.Equal(new GameUpdateCall(gameId, "Título", "Descrição", 19.90m, ReleaseDate), _gameRepository.UpdateCall);
    }

    [Fact]
    public void UpdateGame_WhenGameDoesNotExist_ReturnsFailureWithoutUpdating()
    {
        _gameRepository.GameExistsById = false;

        var result = _service.UpdateGame(new UpdateGameDto(null, "Título", null, null, null), Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Null(_gameRepository.UpdateCall);
    }

    [Fact]
    public void UpdateGame_WhenRepositoryThrows_PropagatesException()
    {
        _gameRepository.ExceptionToThrow = new KeyNotFoundException("not found");

        Assert.Throws<KeyNotFoundException>(
            () => _service.UpdateGame(new UpdateGameDto(null, "Título", null, null, null), Guid.NewGuid()));
    }
}
