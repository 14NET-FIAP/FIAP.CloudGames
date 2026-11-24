using FIAP.CloudGames.API.Attributes;
using FIAP.CloudGames.Application.DTOs.User;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.CloudGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    public UsersController(IUsersService usersService) => _usersService = usersService;

    [HttpPost]
    [AllowAnonymous]
    public IActionResult CreateUser([FromBody] CreateUserDto userDto)
    {
        var result = _usersService.CreateUser(userDto);

        if(!result.IsSuccess)
            return BadRequest(result);

        return Created();
    }

    [HttpGet]
    [IsAdmin]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        ResultList<UserDto> result = await _usersService.GetAllUsersAsync(ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    [IsSelfOrAdmin]
    public IActionResult GetUserById(Guid id)
    {
        Result<UserDto> result = _usersService.GetUserById(id);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    [IsSelfOrAdmin]
    public IActionResult UpdateUser([FromBody] UpdateUserDto userDto, Guid id)
    {
        Result result = _usersService.UpdateUser(id, userDto);

        return result.IsSuccess ? NoContent() : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    [IsSelfOrAdmin]
    public IActionResult DeactivateUser(Guid id)
    {
        Result result = _usersService.DeactivateUser(id);

        return result.IsSuccess ? NoContent() : BadRequest(result);
    }
}
