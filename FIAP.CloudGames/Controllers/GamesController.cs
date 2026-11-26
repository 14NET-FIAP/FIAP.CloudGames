using FIAP.CloudGames.API.Attributes;
using FIAP.CloudGames.Application.DTOs.Games;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.CloudGames.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GamesController : ControllerBase
{
    private readonly IGamesService _gamesService;
    public GamesController(IGamesService gamesService) => _gamesService = gamesService;


    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        ResultList<GamesListDto> result = await _gamesService.GetAllActivesGamesAsync(ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    //[HttpGet("{id}")]
    //public IActionResult GetById(Guid id)
    //{
    //    //Result<GamesDatailDto> result = _gamesService.GetGameById(id);
    //    return Ok($"Game details for ID: {id}");
    //}

    [HttpPost]
    [IsAdmin]
    public IActionResult Create([FromBody] CreateGameDto gameDto)
    {
        Result<Guid> result = _gamesService.CreateNewGame(gameDto);

        return result.IsSuccess 
        ? Created($"api/games/{result.Value}", result.Value) 
        : BadRequest(result);

    }

    [HttpPut("{id}")]
    [IsAdmin]
    public IActionResult Update([FromBody] UpdateGameDto gameDto, Guid id)
    {
        Result result = _gamesService.UpdateGame(gameDto, id);
        return result.IsSuccess ? NoContent() : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [IsAdmin]
    public IActionResult Delete(Guid id)
    {
        Result result = _gamesService.DeleteGame(id);
        return result.IsSuccess ? NoContent() : BadRequest(result);
    }
}
