using FIAP.CloudGames.Application.DTOs.Promotion;
using FIAP.CloudGames.Domain.Entities;
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

    public static Promotion MapPromotionToUpdate(this UpdatePromotionDto promotionDto, Guid id)
    {
        return new Promotion{
            Id = id,
            DiscountPercentage = promotionDto.DiscountPercentage,
            StartDate = promotionDto.StartDate,
            EndDate = promotionDto.EndDate,
            GameId = promotionDto.GameId
        };
    }
}
