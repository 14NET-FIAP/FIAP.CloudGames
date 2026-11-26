using FIAP.CloudGames.Application.DTOs.Promotion;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.Mappings;

public static class PromotionMapping
{
    public static Promotion MapPromotionToCreate(this CreatePromotionDto promotionDto)
    {
        return new Promotion(
            promotionDto.DiscountPercentage,
            promotionDto.StartDate,
            promotionDto.EndDate,
            promotionDto.GameId
        );
    }

    public static PromotionListDto MapPromotionRowToDto(this PromotionListRow promotion)
    {
        return new PromotionListDto(
            promotion.Id,
            promotion.GameId,
            promotion.DiscountPercentage,
            promotion.StartDate,
            promotion.EndDate,
            promotion.IsActive,
            promotion.CreatedAt
        );
    }
}
