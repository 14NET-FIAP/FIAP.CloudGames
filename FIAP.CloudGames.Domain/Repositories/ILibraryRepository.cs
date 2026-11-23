using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;

namespace FIAP.CloudGames.Domain.Repositories;

public interface ILibraryRepository
{
    public Task<IReadOnlyList<UserLibraryItemRow>> GetUserLibraryAsync(Guid userId, CancellationToken ct = default);
}
