using FIAP.CloudGames.API.Attributes;
using FIAP.CloudGames.Application.DTOs.User;
using FIAP.CloudGames.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.CloudGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class UsersController : Controller
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

    //[HttpGet("getPersonalInfo")]
    //[Authorize]
    //public IActionResult GetPersonalInfo()
    //{
    //    return Ok();
    //}

    //[HttpGet("{id}")]
    //[IsAdmin]
    //public IActionResult GetUserById(int id)
    //{
    //    return Ok();
    //}

    //[HttpGet("GetAllUsers")]
    //[IsAdmin]
    //public IActionResult GetAllUsers()
    //{
    //    return Ok();
    //}

    //[HttpPut("{id}")]
    //[Authorize]
    //public IActionResult UpdateUser(int id)
    //{
    //    return Ok();
    //}

    //[HttpDelete("{id}")]
    //[IsAdmin]
    //public IActionResult DeleteUser(int id)
    //{
    //    return Ok();
    //}
}
