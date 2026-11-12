using FIAP.CloudGames.Domain.ValueObjects;
using FIAP.CloudGames.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Application.DTOs.User;

public record CreateUserDto(
    string Name,
    string Email,
    string Password) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("O nome é obrigatório.", [nameof(Name)]);

        var emailError = GetDomainError(() => EmailValueObject.IsValid(Email));
        if (emailError is not null)
            yield return new ValidationResult(emailError, [nameof(Email)]);

        var passwordError = GetDomainError(() => PasswordValueObject.IsValid(Password));
        if (passwordError is not null)
            yield return new ValidationResult(passwordError, [nameof(Password)]);
    }

    private static string? GetDomainError(Action validation)
    {
        try
        {
            validation();
            return null;
        }
        catch (DomainException exception)
        {
            return exception.Message;
        }
    }
}
