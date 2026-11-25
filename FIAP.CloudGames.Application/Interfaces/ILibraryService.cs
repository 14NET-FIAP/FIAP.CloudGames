using FIAP.CloudGames.Application.DTOs.Library;
using FIAP.CloudGames.Domain.Services;

namespace FIAP.CloudGames.Application.Interfaces;

public interface ILibraryService
{
    public Task<ResultList<LibraryListGamesDto>> GetLibraryByUserId(Guid userId, CancellationToken ct = default);
    Task<Result> AddGameToLibraryAsync(Guid userId, AddGameToLibraryDto libraryDto, CancellationToken ct = default);
    Result RemoveGameFromLibrary(Guid userId, Guid gameId);
}
