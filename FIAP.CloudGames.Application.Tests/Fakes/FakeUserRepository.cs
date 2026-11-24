using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;

namespace FIAP.CloudGames.Application.Tests.Fakes;

public sealed class FakeUserRepository : IUserRepository
{
    public bool EmailAlreadyExists { get; set; }
    public bool EmailInUseByAnotherUser { get; set; }
    public Exception? ExceptionToThrowOnCreate { get; set; }
    public Exception? ExceptionToThrowOnUpdate { get; set; }
    public Exception? ExceptionToThrowOnRead { get; set; }
    public User? ActiveUser { get; set; }
    public IReadOnlyList<UserListRow> ActiveUsers { get; set; } = [];

    public User? CreatedUser { get; private set; }
    public User? UpdatedUser { get; private set; }
    public CancellationToken RequestedToken { get; private set; }

    public void CreateUser(User user)
    {
        if (ExceptionToThrowOnCreate is not null)
            throw ExceptionToThrowOnCreate;

        CreatedUser = user;
    }

    public void UpdateUser(User user)
    {
        if (ExceptionToThrowOnUpdate is not null)
            throw ExceptionToThrowOnUpdate;

        UpdatedUser = user;
    }

    public User? GetActiveUserById(Guid id)
    {
        ThrowOnReadIfConfigured();
        return ActiveUser?.Id == id ? ActiveUser : null;
    }

    public Task<IReadOnlyList<UserListRow>> GetAllActiveUsersAsync(CancellationToken ct = default)
    {
        RequestedToken = ct;
        ThrowOnReadIfConfigured();
        return Task.FromResult(ActiveUsers);
    }

    public bool VerifyIfExist(string email) => EmailAlreadyExists;

    public bool VerifyIfExist(string email, Guid ignoredUserId) => EmailInUseByAnotherUser;

    public User? FindLogin(string email, string password) => throw new NotSupportedException();

    private void ThrowOnReadIfConfigured()
    {
        if (ExceptionToThrowOnRead is not null)
            throw ExceptionToThrowOnRead;
    }
}
