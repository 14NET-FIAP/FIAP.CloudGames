using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.DTOs.Library;

public record LibraryListGamesDto(
    Guid GameId,
    string Title,
    string Description,
    decimal PricePaid,
    DateTime? ReleaseDate,
    DateTime AcquisitionDate
);
