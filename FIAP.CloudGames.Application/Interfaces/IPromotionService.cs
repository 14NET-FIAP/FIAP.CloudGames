using FIAP.CloudGames.Application.DTOs.Promotion;
using FIAP.CloudGames.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.Interfaces;

public interface IPromotionService
{
    Task<ResultList<PromotionListDto>> GetAllPromotionsAsync(CancellationToken ct = default);
    Result CreatePromotion(CreatePromotionDto promotionDto);
    Result DesactivePromotion(Guid id);
    Result UpdatePromotion(Guid id, UpdatePromotionDto dto);
}
