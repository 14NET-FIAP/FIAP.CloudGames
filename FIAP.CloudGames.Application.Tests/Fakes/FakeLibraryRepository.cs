using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;

namespace FIAP.CloudGames.Application.Tests.Fakes;

public sealed class FakeLibraryRepository : ILibraryRepository
{
    public IReadOnlyList<UserLibraryItemRow> Rows { get; set; } = [];
    public Exception? ExceptionToThrow { get; set; }
    public Exception? ExceptionToThrowOnWrite { get; set; }
    public bool GameAlreadyInLibrary { get; set; }
    public bool GameWasInLibrary { get; set; } = true;

    public int CallCount { get; private set; }
    public Guid? RequestedUserId { get; private set; }
    public CancellationToken RequestedToken { get; private set; }
    public UserLibrary? AddedItem { get; private set; }
    public (Guid UserId, Guid GameId)? RemovedItem { get; private set; }

    public Task<IReadOnlyList<UserLibraryItemRow>> GetUserLibraryAsync(Guid userId, CancellationToken ct = default)
    {
        CallCount++;
        RequestedUserId = userId;
        RequestedToken = ct;

        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        return Task.FromResult(Rows);
    }

    public void AddGameToLibrary(UserLibrary userLibrary)
    {
        ThrowOnWriteIfConfigured();
        AddedItem = userLibrary;
    }

    public bool RemoveGameFromLibrary(Guid userId, Guid gameId)
    {
        ThrowOnWriteIfConfigured();

        if (!GameWasInLibrary)
            return false;

        RemovedItem = (userId, gameId);
        return true;
    }

    public bool VerifyIfExist(Guid userId, Guid gameId) => GameAlreadyInLibrary;

    private void ThrowOnWriteIfConfigured()
    {
        if (ExceptionToThrowOnWrite is not null)
            throw ExceptionToThrowOnWrite;
    }
}
