using FIAP.CloudGames.API.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.CloudGames.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[IsAdmin]
public class PromotionsController : Controller
{
    [HttpGet]
    public IActionResult GetAllPromotions()
    {
        return Ok("Promotions list");
    }

    [HttpGet("{id}")]
    public IActionResult GetPromotionById(int id)
    {
        return Ok($"Promotion details for ID: {id}");
    }

    [HttpPost]
    public IActionResult CreatePromotion()
    {
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePromotion(int id)
    {
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePromotion(int id)
    {
        return Ok();
    }

}
