using FIAP.CloudGames.Application.DTOs.Library;
using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Application.Tests.DTOs;

public class AddGameToLibraryDtoTests
{
    [Fact]
    public void Validate_WithGameId_IsValid()
    {
        Assert.Empty(Validate(new AddGameToLibraryDto(Guid.NewGuid())));
    }

    [Fact]
    public void Validate_WithEmptyGameId_ReturnsGameIdError()
    {
        var error = Assert.Single(Validate(new AddGameToLibraryDto(Guid.Empty)));

        Assert.Contains(nameof(AddGameToLibraryDto.GameId), error.MemberNames);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(49.90)]
    public void Validate_WithNonNegativePricePaid_IsValid(decimal pricePaid)
    {
        Assert.Empty(Validate(new AddGameToLibraryDto(Guid.NewGuid(), pricePaid)));
    }

    [Fact]
    public void Validate_WithNegativePricePaid_ReturnsPricePaidError()
    {
        var error = Assert.Single(Validate(new AddGameToLibraryDto(Guid.NewGuid(), -0.01m)));

        Assert.Contains(nameof(AddGameToLibraryDto.PricePaid), error.MemberNames);
    }

    private static List<ValidationResult> Validate(AddGameToLibraryDto dto) =>
        dto.Validate(new ValidationContext(dto)).ToList();
}
