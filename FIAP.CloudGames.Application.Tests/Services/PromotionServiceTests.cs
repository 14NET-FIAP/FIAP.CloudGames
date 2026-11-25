using FIAP.CloudGames.Application.Services;
using FIAP.CloudGames.Application.Tests.Fakes;

namespace FIAP.CloudGames.Application.Tests.Services;

public class PromotionServiceTests
{
    private readonly FakePromotionRepository _promotionRepository = new();
    private readonly PromotionService _service;

    public PromotionServiceTests()
    {
        _service = new PromotionService(_promotionRepository);
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
    public void DesactivePromotion_WhenUnexpectedErrorOccurs_PropagatesException()
    {
        _promotionRepository.ExceptionToThrowOnDesactive = new InvalidOperationException("Deadlock na tabela Promotion");

        Assert.Throws<InvalidOperationException>(() => _service.DesactivePromotion(Guid.NewGuid()));
    }

    [Fact]
    public void DesactivePromotion_WhenOperationIsCanceled_PropagatesCancellation()
    {
        _promotionRepository.ExceptionToThrowOnDesactive = new OperationCanceledException();

        Assert.Throws<OperationCanceledException>(() => _service.DesactivePromotion(Guid.NewGuid()));
    }
}
