using FIAP.CloudGames.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Domain.ValueObjects;

public class PasswordValueObject
{
    public static void IsValid(string? password)
    {
        if (string.IsNullOrEmpty(password) 
            || password.Length < 8
            || !password.Any(char.IsLetter) 
            || !password.Any(char.IsDigit)
            || password.All(char.IsLetterOrDigit)
            //|| !password.Any(char.IsUpper)
            )
            throw new DomainException(
                "A senha deve ter no mínimo 8 caracteres, com letras, números e caracteres especiais.");
    }
}
