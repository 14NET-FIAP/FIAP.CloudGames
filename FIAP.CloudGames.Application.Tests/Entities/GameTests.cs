using FIAP.CloudGames.Domain.Entities;

namespace FIAP.CloudGames.Application.Tests.Entities;

public class GameTests
{
    [Theory]
    [InlineData(100, 0, 100)]
    [InlineData(100, 100, 0)]
    [InlineData(59.90, 25, 44.93)]
    [InlineData(0.05, 50, 0.03)] // 0,025 arredonda para cima, como o ROUND do SQL Server
    public void CalculatePriceWithDiscount_ReturnsPriceRoundedToTwoDecimals(decimal price, decimal discount, decimal expected)
    {
        var game = new Game("Celeste", "Plataforma", price, new DateTime(2018, 1, 25));

        Assert.Equal(expected, game.CalculatePriceWithDiscount(discount));
    }
}
