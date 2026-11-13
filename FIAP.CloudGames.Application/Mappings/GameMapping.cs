using FIAP.CloudGames.Application.DTOs.Games;
using FIAP.CloudGames.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.Mappings;

public static class GameMapping
{
    public static Game MapGameToCreate(this CreateGameDto gameDto)
    {
        return new Game(
            gameDto.title,
            gameDto.description,
            gameDto.price,
            gameDto.releaseDate
        );
    }

    public static GamesListDto MapGameToListDto(this Game game)
    {
        return game.MapGameToListDto(game.Price.Value);
    }

    public static GamesListDto MapGameToListDto(this Game game, decimal priceWithDiscount)
    {
        return new GamesListDto(
            game.Id,
            game.Title,
            game.Description,
            game.Price.Value,
            priceWithDiscount,
            game.ReleaseDate.Value
        );
    }

    public static Game MapGameToUpdate(UpdateGameDto gameDto, Guid id)
    {
        return new Game
        {
            Id = id,
            Title = gameDto.title!,             // null = "não alterar"
            Description = gameDto.description!, // null = "não alterar"
            Price = gameDto.price,
            ReleaseDate = gameDto.releaseDate
        };
    }
}
