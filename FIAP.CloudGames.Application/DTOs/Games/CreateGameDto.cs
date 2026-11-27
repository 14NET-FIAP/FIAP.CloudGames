using FIAP.CloudGames.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Application.DTOs.Games;

public record CreateGameDto(
    string title, 
    string description, 
    decimal price, 
    DateTime releaseDate) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(title))
            yield return new ValidationResult(Game.TitleRequiredMessage, [nameof(title)]);

        if (string.IsNullOrWhiteSpace(description))
            yield return new ValidationResult("A descrição é obrigatória", [nameof(description)]);


        if (price < Game.MinPrice)
            yield return new ValidationResult(Game.NegativePriceMessage, [nameof(price)]);
    }
}
