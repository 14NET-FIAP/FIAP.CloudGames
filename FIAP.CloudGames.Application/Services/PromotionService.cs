using FIAP.CloudGames.Application.DTOs.Promotion;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Domain.Services;
using Microsoft.Extensions.Logging;

namespace FIAP.CloudGames.Application.Services;

public class PromotionService : IPromotionService
{
    private const string DesactivePromotionErrorMessage = "Não foi possível desativar a promoção.";

    private readonly IPromotionRepository _promotionRepository;
    private readonly ILogger<PromotionService> _logger;
    public PromotionService(IPromotionRepository promotionService, ILogger<PromotionService> logger)
    {
        _promotionRepository = promotionService;
        _logger = logger;
    }

    public Result CreatePromotion(CreatePromotionDto promotionDto)
    {
        try
        {
            Promotion promotion = PromotionMapping.MapPromotionToCreate(promotionDto);
            _promotionRepository.CreatePromotion(promotion);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao cadastrar a promoção: {ex.Message}");
        }
    }

    public Result DesactivePromotion(Guid id)
    {
        try
        {
            _promotionRepository.DesactivePromotion(id);
            return Result.Success();
        }
        catch (KeyNotFoundException){
            return Result.Failure($"Promoção com ID {id} não encontrada.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException){
            _logger.LogError(ex, "Erro ao desativar a promoção {PromotionId}", id);
            return Result.Failure(DesactivePromotionErrorMessage);
        }
    }

    public Result UpdatePromotion(Guid id, UpdatePromotionDto dto)
    {
        if (!VerifyPromotionExists(id))
            return Result.Failure("Promoção não encontrada.");

        var updatedPromotion = PromotionMapping.MapPromotionToUpdate(dto, id);

        _promotionRepository.UpdateAsync(updatedPromotion);

        return Result.Success();
    }

    private bool VerifyPromotionExists(Guid id) => _promotionRepository.VerifyIfExist(id);

}
