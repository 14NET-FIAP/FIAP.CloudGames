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

    private static List<ValidationResult> Validate(AddGameToLibraryDto dto) =>
        dto.Validate(new ValidationContext(dto)).ToList();
}
