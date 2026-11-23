using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Domain.ReadModels;

public sealed record UserLibraryItemRow(
    Guid GameId,
    string Title,
    string Description,
    decimal PricePaid,
    DateTime? ReleaseDate,
    DateTime AcquisitionDate
);