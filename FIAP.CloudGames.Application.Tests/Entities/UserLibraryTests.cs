using FIAP.CloudGames.Domain.Entities;

namespace FIAP.CloudGames.Application.Tests.Entities;

public class UserLibraryTests
{
    [Fact]
    public void Constructor_StoresPricePaidAndAcquisitionDate()
    {
        Guid userId = Guid.NewGuid();
        Guid gameId = Guid.NewGuid();
        DateTime before = DateTime.UtcNow;

        var item = new UserLibrary(userId, gameId, 44.93m);

        Assert.Equal(userId, item.UserId);
        Assert.Equal(gameId, item.GameId);
        Assert.Equal(44.93m, item.PricePaid);
        Assert.InRange(item.AcquisitionDate, before, DateTime.UtcNow);
    }
}
