using FIAP.CloudGames.Application.DTOs.Promotion;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Exceptions;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Domain.Services;
using Microsoft.Extensions.Logging;

namespace FIAP.CloudGames.Application.Services;

public class PromotionService : IPromotionService
{
    private const string PromotionNotFoundMessage = "Promoção não encontrada.";

    private readonly IPromotionRepository _promotionRepository;
    private readonly ILogger<PromotionService> _logger;

    public PromotionService(IPromotionRepository promotionService, ILogger<PromotionService> logger)
    {
        _promotionRepository = promotionService;
        _logger = logger;
    }

    public async Task<ResultList<PromotionListDto>> GetAllPromotionsAsync(CancellationToken ct = default)
    {
        IReadOnlyList<PromotionListRow> promotions = await _promotionRepository.GetAllPromotionsAsync(ct);

        var dtos = new List<PromotionListDto>(promotions.Count);
        foreach (var promotion in promotions)
            dtos.Add(promotion.MapPromotionRowToDto());

        return ResultList<PromotionListDto>.Success(dtos);
    }

    public Result CreatePromotion(CreatePromotionDto promotionDto)
    {
        try
        {
            Promotion promotion = PromotionMapping.MapPromotionToCreate(promotionDto);
            _promotionRepository.CreatePromotion(promotion);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public Result DesactivePromotion(Guid id)
    {
        Promotion? promotion = _promotionRepository.GetPromotionById(id);
        if (promotion is null)
            return Result.Failure($"Promoção com ID {id} não encontrada.");

        promotion.Deactivate();
        _promotionRepository.UpdatePromotion(promotion);

        _logger.LogInformation("Promoção {PromotionId} desativada", id);
        return Result.Success();
    }

    public Result UpdatePromotion(Guid id, UpdatePromotionDto dto)
    {
        try
        {
            Promotion? promotion = _promotionRepository.GetPromotionById(id);
            if (promotion is null)
                return Result.Failure(PromotionNotFoundMessage);

            promotion.Update(dto.DiscountPercentage, dto.StartDate, dto.EndDate, dto.GameId);
            _promotionRepository.UpdatePromotion(promotion);

            _logger.LogInformation("Promoção {PromotionId} atualizada", id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
