using FIAP.CloudGames.Application.DTOs.User;
using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.Application.Tests.DTOs;

public class UpdateUserDtoTests
{
    [Fact]
    public void Validate_WithAllFieldsNull_IsValid()
    {
        Assert.Empty(Validate(new UpdateUserDto(null, null, null)));
    }

    [Fact]
    public void Validate_WithValidFields_IsValid()
    {
        Assert.Empty(Validate(new UpdateUserDto("Maria", "maria@email.com", "Senha@123")));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithBlankName_ReturnsNameError(string blankName)
    {
        var error = Assert.Single(Validate(new UpdateUserDto(blankName, null, null)));

        Assert.Contains(nameof(UpdateUserDto.Name), error.MemberNames);
    }

    [Theory]
    [InlineData("")]
    [InlineData("maria")]
    [InlineData("maria@email")]
    public void Validate_WithInvalidEmail_ReturnsEmailError(string invalidEmail)
    {
        var error = Assert.Single(Validate(new UpdateUserDto(null, invalidEmail, null)));

        Assert.Contains(nameof(UpdateUserDto.Email), error.MemberNames);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Curta@1")]
    [InlineData("SemNumero@")]
    [InlineData("SemEspecial1")]
    public void Validate_WithWeakPassword_ReturnsPasswordError(string weakPassword)
    {
        var error = Assert.Single(Validate(new UpdateUserDto(null, null, weakPassword)));

        Assert.Contains(nameof(UpdateUserDto.Password), error.MemberNames);
    }

    private static List<ValidationResult> Validate(UpdateUserDto dto) =>
        dto.Validate(new ValidationContext(dto)).ToList();
}
