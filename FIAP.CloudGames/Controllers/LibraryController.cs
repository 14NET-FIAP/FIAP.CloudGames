using FIAP.CloudGames.API.Attributes;
using FIAP.CloudGames.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.CloudGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LibraryController : Controller
{
    private readonly ILibraryService libraryService;
    public LibraryController(ILibraryService libraryService)
    {
        this.libraryService = libraryService;
    }

    [HttpGet]
    public IActionResult GetPersonalLibrary()
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult AddGameToLibrary()
    {
        return Ok();
    }

    [HttpGet("{id}")]
    [IsAdmin]
    public IActionResult GetLibraryByUserId(int id)
    {
        return Ok();
    }
}
