using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.DTOs.Promotion;

public record CreatePromotionDto(
    decimal DiscountPercentage,
    DateTime StartDate,
    DateTime EndDate,
    Guid GameId) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate < StartDate)
            yield return new ValidationResult("A data final não pode ser anterior à data inicial.", [nameof(EndDate)]);

        if (DiscountPercentage is < 0 or > 100)
            yield return new ValidationResult("O desconto deve estar entre 0 e 100.", [nameof(DiscountPercentage)]);
    }
}


