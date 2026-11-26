using FIAP.CloudGames.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FIAP.CloudGames.Domain.ReadModels;

namespace FIAP.CloudGames.Infra.Security;

public static class SecurityHelper
{
    public static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }


    public static string GenerateJwtToken(UserContextService userData, IConfiguration configuration)
    {
        DateTime expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["Jwt:ExpirationInDays"]));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userData.UserId.ToString()),
            new Claim(ClaimTypes.Actor, userData.UserType.ToString()),
            new Claim(ClaimTypes.Expiration, expires.ToString())
        };
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
