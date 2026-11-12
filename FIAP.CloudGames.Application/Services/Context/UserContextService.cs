using FIAP.CloudGames.Domain.Enums;
using FIAP.CloudGames.Domain.Services;

namespace FIAP.CloudGames.Application.Services.Context;

public class UserContextService : IUserContextService
{
    public Guid UserId { get; set; }
    public TypeUserEnum UserType { get; set; }

    public UserContextService(Guid userId, TypeUserEnum userType)
    {
        UserId = userId;
        UserType = userType;
    }
}
