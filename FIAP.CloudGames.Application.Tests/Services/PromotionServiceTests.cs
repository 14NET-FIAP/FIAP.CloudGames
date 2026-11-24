using FIAP.CloudGames.Application.Services;
using FIAP.CloudGames.Application.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace FIAP.CloudGames.Application.Tests.Services;

public class PromotionServiceTests
{
    private readonly FakePromotionRepository _promotionRepository = new();
    private readonly PromotionService _service;

    public PromotionServiceTests()
    {
        _service = new PromotionService(_promotionRepository, NullLogger<PromotionService>.Instance);
    }

    [Fact]
    public void DesactivePromotion_WhenPromotionExists_ReturnsSuccess()
    {
        Guid promotionId = Guid.NewGuid();

        var result = _service.DesactivePromotion(promotionId);

        Assert.True(result.IsSuccess);
        Assert.Equal(promotionId, _promotionRepository.DesactivatedPromotionId);
    }

    [Fact]
    public void DesactivePromotion_WhenPromotionDoesNotExist_ReturnsNotFoundMessage()
    {
        Guid promotionId = Guid.NewGuid();
        _promotionRepository.ExceptionToThrowOnDesactive = new KeyNotFoundException();

        var result = _service.DesactivePromotion(promotionId);

        Assert.False(result.IsSuccess);
        Assert.Equal($"Promoção com ID {promotionId} não encontrada.", result.Message);
    }

    [Fact]
    public void DesactivePromotion_WhenUnexpectedErrorOccurs_DoesNotExposeExceptionDetails()
    {
        var exception = new InvalidOperationException("Deadlock na tabela Promotion");
        _promotionRepository.ExceptionToThrowOnDesactive = exception;

        var result = _service.DesactivePromotion(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível desativar a promoção.", result.Message);
        Assert.DoesNotContain(exception.Message, result.Message);
        Assert.DoesNotContain(nameof(InvalidOperationException), result.Message);
    }

    [Fact]
    public void DesactivePromotion_WhenOperationIsCanceled_PropagatesCancellation()
    {
        _promotionRepository.ExceptionToThrowOnDesactive = new OperationCanceledException();

        Assert.Throws<OperationCanceledException>(() => _service.DesactivePromotion(Guid.NewGuid()));
    }
}
