using FIAP.CloudGames.Application.DTOs.User;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Domain.Services;
using FIAP.CloudGames.Infra.Security;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _usersRepository;
    private readonly IConfiguration _configuration;
    public AuthService(IUserRepository usersRepository, IConfiguration configuration)
    {
        _usersRepository = usersRepository;
        _configuration = configuration;
    }
    public Result<string> Login(LoginDto login)
    {
        var hashPassword = SecurityHelper.HashPassword(login.Password);
        
        var user = _usersRepository.FindLogin(login.Email, hashPassword);
        if(user != null)
        {
            var newUser = UserMapping.MapUserToJWT(user);
            return Result<string>.Success(SecurityHelper.GenerateJwtToken(newUser, _configuration));
        }

        return Result<string>.Failure("Usuário ou senha incorretos");
    }
}
