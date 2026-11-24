using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;

namespace FIAP.CloudGames.Domain.Repositories;

public interface IUserRepository
{
    public void CreateUser(User user);
    public User? FindLogin(string email, string password);
    bool VerifyIfExist(string email);
    bool VerifyIfExist(string email, Guid ignoredUserId);
    Task<IReadOnlyList<UserListRow>> GetAllActiveUsersAsync(CancellationToken ct = default);
    User? GetActiveUserById(Guid id);
    void UpdateUser(User user);
}
