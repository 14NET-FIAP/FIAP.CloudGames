using FIAP.CloudGames.Application.DTOs.Promotion;
using FIAP.CloudGames.Application.Services;
using FIAP.CloudGames.Application.Tests.Fakes;
using FIAP.CloudGames.Domain.ReadModels;

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
    public async Task GetAllPromotionsAsync_ReturnsMappedPromotionsPreservingRepositoryOrder()
    {
        PromotionListRow active = CreatePromotionRow(isActive: true, new DateTime(2025, 2, 1));
        PromotionListRow inactive = CreatePromotionRow(isActive: false, new DateTime(2025, 3, 1));
        _promotionRepository.AllPromotions = [active, inactive];

        var result = await _service.GetAllPromotionsAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(
            [ToDto(active), ToDto(inactive)],
            result.Value);
    }

    [Fact]
    public async Task GetAllPromotionsAsync_WhenThereAreNoPromotions_ReturnsEmptySuccess()
    {
        var result = await _service.GetAllPromotionsAsync();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAllPromotionsAsync_ForwardsCancellationToken()
    {
        using var cancellationSource = new CancellationTokenSource();

        await _service.GetAllPromotionsAsync(cancellationSource.Token);

        Assert.Equal(cancellationSource.Token, _promotionRepository.RequestedAllPromotionsToken);
    }

    [Fact]
    public async Task GetAllPromotionsAsync_WhenRepositoryThrows_PropagatesException()
    {
        _promotionRepository.ExceptionToThrowOnRead = new InvalidOperationException("Timeout expired on SqlConnection");

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetAllPromotionsAsync());
    }

    [Fact]
    public async Task GetAllPromotionsAsync_WhenOperationIsCanceled_PropagatesCancellation()
    {
        _promotionRepository.ExceptionToThrowOnRead = new OperationCanceledException();

        await Assert.ThrowsAsync<OperationCanceledException>(() => _service.GetAllPromotionsAsync());
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

    private static PromotionListRow CreatePromotionRow(bool isActive, DateTime createdAt) => new()
    {
        Id = Guid.NewGuid(),
        GameId = Guid.NewGuid(),
        DiscountPercentage = 20m,
        StartDate = new DateTime(2025, 1, 1),
        EndDate = new DateTime(2025, 12, 31),
        IsActive = isActive,
        CreatedAt = createdAt
    };

    private static PromotionListDto ToDto(PromotionListRow row) =>
        new(row.Id, row.GameId, row.DiscountPercentage, row.StartDate, row.EndDate, row.IsActive, row.CreatedAt);
}
