using FIAP.CloudGames.Application.DTOs.User;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Exceptions;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Domain.Services;
using FIAP.CloudGames.Infra.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUserRepository _usersRepository;
    public UsersService(IUserRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }


    public Result CreateUser(CreateUserDto userDto)
    {
        try
        {
            User user = UserMapping.MapUserToCreate(userDto);

            if (VerifyIfExist(user.Email))
                return Result.Failure("Usuário já cadastrado no sistema");

            _usersRepository.CreateUser(user);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }        
    }

    private bool VerifyIfExist(string email) => _usersRepository.VerifyIfExist(email);
}
