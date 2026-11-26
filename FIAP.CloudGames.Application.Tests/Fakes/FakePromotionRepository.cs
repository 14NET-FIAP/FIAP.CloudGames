using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;

namespace FIAP.CloudGames.Application.Tests.Fakes;

public sealed class FakePromotionRepository : IPromotionRepository
{
    public Exception? ExceptionToThrowOnDesactive { get; set; }
    public Exception? ExceptionToThrowOnRead { get; set; }

    public IReadOnlyList<Promotion> ActivePromotions { get; set; } = [];
    public IReadOnlyList<PromotionListRow> AllPromotions { get; set; } = [];

    public Guid? DesactivatedPromotionId { get; private set; }
    public CancellationToken RequestedPromotionsToken { get; private set; }
    public CancellationToken RequestedAllPromotionsToken { get; private set; }

    public void DesactivePromotion(Guid id)
    {
        if (ExceptionToThrowOnDesactive is not null)
            throw ExceptionToThrowOnDesactive;

        DesactivatedPromotionId = id;
    }

    public void CreatePromotion(Promotion promotion) => throw new NotSupportedException();

    public Task<IReadOnlyList<PromotionListRow>> GetAllPromotionsAsync(CancellationToken ct = default)
    {
        RequestedAllPromotionsToken = ct;

        if (ExceptionToThrowOnRead is not null)
            throw ExceptionToThrowOnRead;

        return Task.FromResult(AllPromotions);
    }

    public IEnumerable<Promotion> GetActivePromotions(DateTime currentDate) => throw new NotSupportedException();

    public Task<IReadOnlyList<Promotion>> GetActivePromotionsByGameIdsAsync(IEnumerable<Guid> gameIds, CancellationToken ct = default)
    {
        RequestedPromotionsToken = ct;
        return Task.FromResult(ActivePromotions);
    }

    public void UpdateAsync(Promotion updatedPromotion) => throw new NotSupportedException();

    public bool VerifyIfExist(Guid id) => throw new NotSupportedException();
}
