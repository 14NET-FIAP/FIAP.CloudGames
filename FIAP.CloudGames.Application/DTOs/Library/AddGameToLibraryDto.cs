using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Application.DTOs.Library;

// PricePaid opcional: quando omitido, o service calcula o preço atual com o maior desconto ativo.
public record AddGameToLibraryDto(Guid GameId, decimal? PricePaid = null) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (GameId == Guid.Empty)
            yield return new ValidationResult("O identificador do jogo é obrigatório.", [nameof(GameId)]);

        if (PricePaid < 0)
            yield return new ValidationResult("O preço pago não pode ser negativo.", [nameof(PricePaid)]);
    }
}
