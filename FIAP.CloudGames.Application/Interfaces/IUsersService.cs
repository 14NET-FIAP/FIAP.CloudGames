using FIAP.CloudGames.Application.DTOs.User;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.Interfaces;

public interface IUsersService
{
    public Result CreateUser(CreateUserDto userDto);
}
