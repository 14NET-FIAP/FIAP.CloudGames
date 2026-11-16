using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.DTOs.Games;

public record GamesDatailDto : GamesListDto
{
    public GamesDatailDto(
        Guid id,
        string title,
        string description,
        decimal originalPrice,
        decimal priceWithDiscount,
        DateTime releaseDate
    ) : base(id, title, description, originalPrice, priceWithDiscount, releaseDate)
    {
    }
}