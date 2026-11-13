namespace FIAP.CloudGames.Application.DTOs.Games;

public record UpdateGameDto(
    Guid? id,
    string? title,
    string? description,
    decimal? price,
    DateTime? releaseDate);
