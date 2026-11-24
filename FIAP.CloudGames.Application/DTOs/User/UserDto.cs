using FIAP.CloudGames.Domain.Enums;

namespace FIAP.CloudGames.Application.DTOs.User;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    TypeUserEnum TypeUser,
    DateTime CreatedAt);
