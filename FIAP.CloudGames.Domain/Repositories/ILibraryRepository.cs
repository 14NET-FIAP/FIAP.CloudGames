using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;

namespace FIAP.CloudGames.Domain.Repositories;

public interface ILibraryRepository
{
    public Task<IReadOnlyList<UserLibraryItemRow>> GetUserLibraryAsync(Guid userId, CancellationToken ct = default);
    void AddGameToLibrary(UserLibrary userLibrary);
    // Retorna false quando o jogo não estava na biblioteca do usuário.
    bool RemoveGameFromLibrary(Guid userId, Guid gameId);
    bool VerifyIfExist(Guid userId, Guid gameId);
}
