using FIAP.CloudGames.API.Attributes;
using FIAP.CloudGames.Application.DTOs.User;
using FIAP.CloudGames.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.CloudGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    public UsersController(IUsersService usersService) => _usersService = usersService;

    [HttpPost("register")]
    public IActionResult CreateUser([FromBody] CreateUserDto userDto)
    {
        var result = _usersService.CreateUser(userDto);

        if(!result.IsSuccess)
            return BadRequest(result);

        return Created();
    }

    //[HttpPut]
    //public IActionResult UpdateUser()
    //{

    //}
}
