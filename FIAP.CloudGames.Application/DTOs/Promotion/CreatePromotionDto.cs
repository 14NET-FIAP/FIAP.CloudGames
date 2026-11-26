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
        if (!Domain.Entities.Promotion.IsValidPeriod(StartDate, EndDate))
            yield return new ValidationResult(Domain.Entities.Promotion.InvalidPeriodMessage, [nameof(EndDate)]);

        if (!Domain.Entities.Promotion.IsValidDiscount(DiscountPercentage))
            yield return new ValidationResult(Domain.Entities.Promotion.InvalidDiscountMessage, [nameof(DiscountPercentage)]);
    }
}


