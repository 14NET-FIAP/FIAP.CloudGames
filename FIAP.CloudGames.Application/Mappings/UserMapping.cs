using FIAP.CloudGames.Application.DTOs.User;
using FIAP.CloudGames.Application.Services.Context;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Infra.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.Mappings;

public static class UserMapping
{
    public static User MapUserToCreate(this CreateUserDto userDto)
    {
        return new User(
            userDto.Name,
            userDto.Email,
            SecurityHelper.HashPassword(userDto.Password)
        );
    }

    public static UserDto MapUserToDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.TypeUser,
            user.CreatedAt
        );
    }

    public static UserDto MapUserRowToDto(this UserListRow user)
    {
        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.TypeUser,
            user.CreatedAt
        );
    }

    public static UserContextService MapUserToJWT(this User user)
    {
        return new UserContextService(
            user.Id,
            user.TypeUser
        );
    }
}
