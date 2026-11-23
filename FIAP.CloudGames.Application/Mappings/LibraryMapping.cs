using FIAP.CloudGames.Application.DTOs.Games;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIAP.CloudGames.Application.DTOs.Library;

namespace FIAP.CloudGames.Application.Mappings;

public static class LibraryMapping
{
    public static LibraryListGamesDto MapLibraryRowToDto(this UserLibraryItemRow userLibrary)
    {
        return new LibraryListGamesDto(
            userLibrary.GameId,
            userLibrary.Title,
            userLibrary.Description,
            userLibrary.PricePaid,
            userLibrary.ReleaseDate,
            userLibrary.AcquisitionDate
        );
    }
}