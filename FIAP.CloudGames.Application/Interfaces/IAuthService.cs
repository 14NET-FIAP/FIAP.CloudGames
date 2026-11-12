using FIAP.CloudGames.Application.DTOs.User;
using FIAP.CloudGames.Domain.Services;

namespace FIAP.CloudGames.Application.Interfaces;

public interface IAuthService
{
    public Result<string> Login(LoginDto login);
}
