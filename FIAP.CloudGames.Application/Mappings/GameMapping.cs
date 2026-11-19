using FIAP.CloudGames.Application.DTOs.Games;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
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


    public static GamesListDto MapGameToDatailDto(this Game game, decimal priceWithDiscount)
    {
        return new GamesListDto(
            game.Id,
            game.Title,
            game.Description,
            game.Price,
            priceWithDiscount,
            game.ReleaseDate!.Value
        );
    }

    public static GamesListDto MapGameToListDto(this GameListRow game)
    {
        return new GamesListDto(
            game.Id,
            game.Title,
            game.Description,
            game.OriginalPrice,
            game.FinalPrice,
            game.ReleaseDate
        );
    }

    public static Game MapGameToUpdate(UpdateGameDto gameDto, Guid id)
    {
        return new Game(
            id,
            gameDto.title!, 
            gameDto.description!,
            gameDto.price.Value,
            gameDto.releaseDate.Value
        );
    }
}