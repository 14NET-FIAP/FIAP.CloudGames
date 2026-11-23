using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.DTOs.User;

public record UpdateUserDto(
    string Name,
    string Email,
    string Password);