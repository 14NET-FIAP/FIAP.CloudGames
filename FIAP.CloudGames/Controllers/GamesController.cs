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
public class GamesController : Controller
{
    private readonly IGamesService _gamesService;
    public GamesController(IGamesService gamesService) => _gamesService = gamesService;


    [HttpGet]
    public IActionResult Get()
    {
        ResultList<GamesListDto> result = _gamesService.GetAllGames();
        return Ok("Games list");
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok($"Game details for ID: {id}");
    }

    [HttpPost]
    [IsAdmin]
    public IActionResult Create([FromBody] CreateGameDto gameDto)
    {
        Result result = _gamesService.CreateNewGame(gameDto);

        if(!result.IsSuccess)
            return BadRequest(result);

        return Created();
    }

    [HttpPut("{id}")]
    [IsAdmin]
    public IActionResult Update([FromBody] UpdateGameDto gameDto, Guid id)
    {
        Result result = _gamesService.UpdateGame(gameDto, id);

        if(!result.IsSuccess)
            return BadRequest(result);
        return Ok();
    }

    [HttpDelete("{id}")]
    [IsAdmin]
    public IActionResult Delete(int id)
    {
        return Ok();
    }
}
