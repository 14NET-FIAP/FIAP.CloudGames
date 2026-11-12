using FIAP.CloudGames.API.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.CloudGames.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GamesController : Controller
{

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Games list");
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok($"Game details for ID: {id}");
    }

    [HttpPost]
    [IsAdmin]
    public IActionResult Create()
    {
        return Ok();
    }

    [HttpPut("{id}")]
    [IsAdmin]
    public IActionResult Update(int id)
    {
        return Ok();
    }

    [HttpDelete("{id}")]
    [IsAdmin]
    public IActionResult Delete(int id)
    {
        return Ok();
    }
}
