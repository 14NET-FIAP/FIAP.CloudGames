using FIAP.CloudGames.Domain.Enums;

namespace FIAP.CloudGames.Infra.Security;

public record JwtUserClaims(Guid UserId, TypeUserEnum UserType);
