using FIAP.CloudGames.API.Attributes;
using FIAP.CloudGames.Application.DTOs.Promotion;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.CloudGames.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[IsAdmin]
public class PromotionsController : Controller
{
    private readonly IPromotionService _promotionService;
    public PromotionsController(IPromotionService promotionService) => _promotionService = promotionService;

    //[HttpGet]
    //public IActionResult GetAllPromotions()
    //{
    //    return Ok("Promotions list");
    //}

    //[HttpGet("{id}")]
    //public IActionResult GetPromotionById(int id)
    //{
    //    return Ok($"Promotion details for ID: {id}");
    //}

    [HttpPost]
    public IActionResult CreatePromotion([FromBody] CreatePromotionDto promotionDto)
    {
        Result result = _promotionService.CreatePromotion(promotionDto);
        if (!result.IsSuccess)
            return BadRequest(result);

        return NoContent();
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePromotion([FromBody]UpdatePromotionDto promotionDto, Guid id)
    {
        Result result = _promotionService.UpdatePromotion(id, promotionDto);
        if (!result.IsSuccess)
            return BadRequest(result);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePromotion(Guid id)
    {
        Result result = _promotionService.DesactivePromotion(id);
        if (!result.IsSuccess)
            return BadRequest(result);

        return NoContent();
    }

}
