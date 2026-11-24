using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Application.DTOs.Library;

public record AddGameToLibraryDto(Guid GameId) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (GameId == Guid.Empty)
            yield return new ValidationResult("O identificador do jogo é obrigatório.", [nameof(GameId)]);
    }
}
