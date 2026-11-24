using FIAP.CloudGames.API.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FIAP.CloudGames.Application.DTOs.Library;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Domain.Services;
using System.Security.Claims;

namespace FIAP.CloudGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LibraryController : Controller
{
    private readonly ILibraryService _libraryService;
    public LibraryController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPersonalLibrary(CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ResultList<LibraryListGamesDto> result = await _libraryService.GetLibraryByUserId(userId!, ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddGameToPersonalLibrary([FromBody] AddGameToLibraryDto libraryDto, CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Result result = await _libraryService.AddGameToLibraryAsync(userId!, libraryDto, ct);

        return result.IsSuccess ? Created() : BadRequest(result);
    }

    [HttpPost("{id}")]
    [IsAdmin]
    public async Task<IActionResult> AddGameToLibrary(string id, [FromBody] AddGameToLibraryDto libraryDto, CancellationToken ct)
    {
        Result result = await _libraryService.AddGameToLibraryAsync(id, libraryDto, ct);

        return result.IsSuccess ? Created() : BadRequest(result);
    }

    [HttpDelete("{gameId}/{userId}")]
    [IsAdmin]
    public IActionResult RemoveGameFromLibrary(Guid gameId, Guid userId)
    {
        Result result = _libraryService.RemoveGameFromLibrary(userId, gameId);

        return result.IsSuccess ? NoContent() : BadRequest(result);
    }

    [HttpGet("{id}")]
    [IsAdmin]
    public async Task<IActionResult> GetLibraryByUserId(string id, CancellationToken ct)
    {
        ResultList<LibraryListGamesDto> result = await _libraryService.GetLibraryByUserId(id, ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }
}
