using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;

namespace FIAP.CloudGames.Application.Tests.Fakes;

public sealed record GameUpdateCall(Guid Id, string? Title, string? Description, decimal? Price, DateTime? ReleaseDate);

public sealed class FakeGameRepository : IGameRepository
{
    public bool GameExistsByTitleAndDate { get; set; }
    public bool GameExistsById { get; set; } = true;
    public Exception? ExceptionToThrow { get; set; }
    public Game? GameById { get; set; }

    public Game? CreatedGame { get; private set; }
    public GameUpdateCall? UpdateCall { get; private set; }

    public void CreateGame(Game game)
    {
        ThrowIfConfigured();
        CreatedGame = game;
    }

    public void UpdateGame(Guid id, string? title, string? description, decimal? price, DateTime? releaseDate)
    {
        ThrowIfConfigured();
        UpdateCall = new GameUpdateCall(id, title, description, price, releaseDate);
    }

    public bool VerifyIfExist(string name, DateTime releaseDate) => GameExistsByTitleAndDate;

    public bool VerifyIfExist(Guid id) => GameExistsById;

    public IEnumerable<Game> GetAllActivesGames() => throw new NotSupportedException();

    public Task<IReadOnlyList<Game>> GetAllActivesGamesAsync(CancellationToken ct = default) => throw new NotSupportedException();

    public Task<IReadOnlyList<GameListRow>> GetActiveGamesWithBestDiscountAsync(CancellationToken ct = default) => throw new NotSupportedException();

    public Game? GetGameById(Guid id) => GameById?.Id == id ? GameById : null;
    public void DeleteGame(Guid id) => throw new NotSupportedException();

    private void ThrowIfConfigured()
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;
    }
}
