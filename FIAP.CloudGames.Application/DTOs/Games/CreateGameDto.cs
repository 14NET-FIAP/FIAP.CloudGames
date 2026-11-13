using FIAP.CloudGames.Domain.Exceptions;
using FIAP.CloudGames.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            yield return new ValidationResult("O título é obrigatório.", [nameof(title)]);

        if (string.IsNullOrWhiteSpace(description))
            yield return new ValidationResult("A descrição é obrigatória", [nameof(description)]);

        if(releaseDate == null)
            yield return new ValidationResult("A data de lançamento não pode ser nula", [nameof(releaseDate)]);

        if (price <= 0)
            yield return new ValidationResult("O Preço do jogo não pode ser negativo", [nameof(price)]);
    }
}
