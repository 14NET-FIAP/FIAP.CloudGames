using FIAP.CloudGames.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Domain.ValueObjects;

public class EmailValueObject
{
    private static readonly Regex Pattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }
    public EmailValueObject(string value) => Value = value;

    public static EmailValueObject IsValid(string? value)
    {
        var email = value?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || !Pattern.IsMatch(email))
            throw new DomainException("O formato do e-mail é inválido.");
        return new EmailValueObject(email);
    }
}
