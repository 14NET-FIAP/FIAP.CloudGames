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
    private const string InvalidUserIdMessage = "Identificador de usuário inválido.";

    private readonly ILibraryService _libraryService;
    private readonly ILogger<LibraryController> _logger;
    public LibraryController(ILibraryService libraryService, ILogger<LibraryController> logger)
    {
        _libraryService = libraryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetPersonalLibrary(CancellationToken ct)
    {
        if (!TryGetAuthenticatedUserId(out Guid userId))
            return InvalidUserIdResult();

        ResultList<LibraryListGamesDto> result = await _libraryService.GetLibraryByUserId(userId, ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddGameToPersonalLibrary([FromBody] AddGameToLibraryDto libraryDto, CancellationToken ct)
    {
        if (!TryGetAuthenticatedUserId(out Guid userId))
            return InvalidUserIdResult();

        Result result = await _libraryService.AddGameToLibraryAsync(userId, libraryDto, ct);

        return result.IsSuccess ? Created() : BadRequest(result);
    }

    [HttpPost("{id}")]
    [IsAdmin]
    public async Task<IActionResult> AddGameToLibrary(Guid id, [FromBody] AddGameToLibraryDto libraryDto, CancellationToken ct)
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
    public async Task<IActionResult> GetLibraryByUserId(Guid id, CancellationToken ct)
    {
        ResultList<LibraryListGamesDto> result = await _libraryService.GetLibraryByUserId(id, ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    private bool TryGetAuthenticatedUserId(out Guid userId) =>
        Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);

    private BadRequestObjectResult InvalidUserIdResult()
    {
        // O token é emitido pela própria API; claim ausente/inválida indica token adulterado ou de outra versão.
        _logger.LogWarning("Requisição {Method} {Path} recusada: claim de identificador de usuário ausente ou inválida",
            Request.Method, Request.Path);
        return BadRequest(Result.Failure(InvalidUserIdMessage));
    }
}
