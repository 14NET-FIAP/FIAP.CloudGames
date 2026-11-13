using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.DTOs.Games;

public record GamesListDto(
    Guid id,
    string title,
    string description,
    decimal originalPrice,
    decimal priceWithDiscount,
    DateTime releaseDate
    );
