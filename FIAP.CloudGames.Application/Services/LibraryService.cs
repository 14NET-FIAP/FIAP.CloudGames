using FIAP.CloudGames.Application.DTOs.Games;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Domain.Services;
using FIAP.CloudGames.Application.DTOs.Library;

namespace FIAP.CloudGames.Application.Services;

public class LibraryService : ILibraryService
{
    private readonly ILibraryRepository _libraryRepository;
    // private readonly IPromotionRepository _promotionRepository;

    public LibraryService(ILibraryRepository libraryRepository)
    {
        _libraryRepository = libraryRepository;
    }

    public async Task<ResultList<LibraryListGamesDto>> GetLibraryByUserId(string UserId, CancellationToken ct = default)
    {
        try
        {
            var result = await _libraryRepository.GetUserLibraryAsync(new Guid(UserId), ct);

            var dtos = new List<LibraryListGamesDto>(result.Count);
            foreach (var userLibraryItem in result)
                dtos.Add(userLibraryItem.MapLibraryRowToDto());

            return ResultList<LibraryListGamesDto>.Success();
        }
        catch (Exception ex)
        {
            return ResultList<LibraryListGamesDto>.Failure($"Erro ao buscar Lib de jogos: {ex.Message}");
        }

    }
}