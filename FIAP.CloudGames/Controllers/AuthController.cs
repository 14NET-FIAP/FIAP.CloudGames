using FIAP.CloudGames.Application.DTOs.User;
using FIAP.CloudGames.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.CloudGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto login)
    {
        var result = _authService.Login(login);

        return result.IsSuccess ? Ok(result) : Unauthorized(result);

    }

    //[HttpPost("RefreshToken")]
    //[Authorize]
    //public IActionResult RefreshToken()
    //{
    //     new NotImplementedException();
    //    return Ok();
    //}
}
