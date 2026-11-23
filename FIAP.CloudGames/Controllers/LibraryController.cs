using FIAP.CloudGames.API.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FIAP.CloudGames.Application.DTOs.Library;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Domain.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace FIAP.CloudGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LibraryController : Controller
{
    private readonly ILibraryService _libraryService;
    private readonly ActionExecutingContext _context;
    public LibraryController(ILibraryService libraryService, ActionExecutingContext context)
    {
        _libraryService = libraryService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPersonalLibrary()
    {
        var userId = _context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ResultList<LibraryListGamesDto> result = await _libraryService.GetLibraryByUserId(userId!);
        
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public IActionResult AddGameToLibrary()
    {
        return Ok();
    }

    [HttpDelete("{gameId}/{userId}")]
    [IsAdmin]
    public IActionResult UpdateGameToLibrary(Guid gameId, Guid userId)
    {
        return Ok();
    }

    [HttpGet("{id}")]
    [IsAdmin]
    public IActionResult GetLibraryByUserId(string id)
    {
        var result = _libraryService.GetLibraryByUserId(id);
        return Ok();
    }
}
