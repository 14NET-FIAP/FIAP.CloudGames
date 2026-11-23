using FIAP.CloudGames.Application.DTOs.Library;
using FIAP.CloudGames.Domain.Services;

namespace FIAP.CloudGames.Application.Interfaces;

public interface ILibraryService
{
    public Task<ResultList<LibraryListGamesDto>> GetLibraryByUserId(string UserId, CancellationToken ct = default);
}
