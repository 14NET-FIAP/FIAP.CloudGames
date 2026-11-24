using FIAP.CloudGames.Domain.Exceptions;
using FIAP.CloudGames.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Application.DTOs.User;

// Atualização parcial: campos nulos mantêm o valor atual; os enviados seguem as regras do cadastro.
public record UpdateUserDto(
    string? Name,
    string? Email,
    string? Password) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Name is not null && string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("O nome não pode ser vazio.", [nameof(Name)]);

        if (Email is not null)
        {
            var emailError = GetDomainError(() => EmailValueObject.IsValid(Email));
            if (emailError is not null)
                yield return new ValidationResult(emailError, [nameof(Email)]);
        }

        if (Password is not null)
        {
            var passwordError = GetDomainError(() => PasswordValueObject.IsValid(Password));
            if (passwordError is not null)
                yield return new ValidationResult(passwordError, [nameof(Password)]);
        }
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
