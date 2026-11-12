using FIAP.CloudGames.Domain.Entities;

namespace FIAP.CloudGames.Domain.Repositories;

public interface IUserRepository
{
    public void CreateUser(User user);
    public User? FindLogin(string email, string password);
    bool VerifyIfExist(string email);
}
