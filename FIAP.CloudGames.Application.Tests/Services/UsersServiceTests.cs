using FIAP.CloudGames.Application.DTOs.User;
using FIAP.CloudGames.Application.Services;
using FIAP.CloudGames.Application.Tests.Fakes;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Enums;
using FIAP.CloudGames.Domain.Exceptions;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Infra.Security;
using Microsoft.Extensions.Logging.Abstractions;

namespace FIAP.CloudGames.Application.Tests.Services;

public class UsersServiceTests
{
    private const string UserAlreadyExistsMessage = "Usuário já cadastrado no sistema";
    private const string EmailInUseMessage = "O e-mail informado já está em uso por outro usuário.";
    private const string UserNotFoundMessage = "Usuário não encontrado.";
    private const string TechnicalErrorDetail = "Cannot open database FIAP_CloudGames";
    private static readonly CreateUserDto ValidUser = new("Maria", "maria@email.com", "Senha@123");

    private readonly FakeUserRepository _userRepository = new();
    private readonly UsersService _service;

    public UsersServiceTests()
    {
        _service = new UsersService(_userRepository, NullLogger<UsersService>.Instance);
    }

    [Fact]
    public void CreateUser_WithNewEmail_CreatesUser()
    {
        var result = _service.CreateUser(ValidUser);

        Assert.True(result.IsSuccess);
        Assert.NotNull(_userRepository.CreatedUser);
        Assert.Equal(ValidUser.Email, _userRepository.CreatedUser!.Email);
    }

    [Fact]
    public void CreateUser_WhenEmailAlreadyExists_ReturnsFailureWithoutCreating()
    {
        _userRepository.EmailAlreadyExists = true;

        var result = _service.CreateUser(ValidUser);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserAlreadyExistsMessage, result.Message);
        Assert.Null(_userRepository.CreatedUser);
    }

    [Fact]
    public void CreateUser_WhenConcurrentInsertViolatesUniqueEmail_ReturnsUserAlreadyExists()
    {
        _userRepository.ExceptionToThrowOnCreate =
            new DuplicateEntityException("Já existe um usuário com o e-mail informado.", new Exception("2627"));

        var result = _service.CreateUser(ValidUser);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserAlreadyExistsMessage, result.Message);
    }

    [Fact]
    public void CreateUser_WhenDomainRuleFails_ReturnsDomainMessage()
    {
        _userRepository.ExceptionToThrowOnCreate = new DomainException("O formato do e-mail é inválido.");

        var result = _service.CreateUser(ValidUser);

        Assert.False(result.IsSuccess);
        Assert.Equal("O formato do e-mail é inválido.", result.Message);
    }

    [Fact]
    public void CreateUser_WhenUnexpectedErrorOccurs_ReturnsGenericFailureWithoutTechnicalDetail()
    {
        _userRepository.ExceptionToThrowOnCreate = new InvalidOperationException(TechnicalErrorDetail);

        var result = _service.CreateUser(ValidUser);

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível cadastrar o usuário.", result.Message);
    }

    [Fact]
    public void CreateUser_WhenOperationIsCanceled_PropagatesCancellation()
    {
        _userRepository.ExceptionToThrowOnCreate = new OperationCanceledException();

        Assert.Throws<OperationCanceledException>(() => _service.CreateUser(ValidUser));
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsMappedActiveUsers()
    {
        var row = new UserListRow { Id = Guid.NewGuid(), Name = "Maria", Email = "maria@email.com", TypeUser = TypeUserEnum.User, CreatedAt = new DateTime(2025, 1, 10) };
        _userRepository.ActiveUsers = [row];

        var result = await _service.GetAllUsersAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(new UserDto(row.Id, row.Name, row.Email, row.TypeUser, row.CreatedAt), Assert.Single(result.Value));
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenThereAreNoUsers_ReturnsEmptySuccess()
    {
        var result = await _service.GetAllUsersAsync();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAllUsersAsync_ForwardsCancellationToken()
    {
        using var cancellationSource = new CancellationTokenSource();

        await _service.GetAllUsersAsync(cancellationSource.Token);

        Assert.Equal(cancellationSource.Token, _userRepository.RequestedToken);
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenRepositoryThrows_ReturnsGenericFailure()
    {
        _userRepository.ExceptionToThrowOnRead = new InvalidOperationException(TechnicalErrorDetail);

        var result = await _service.GetAllUsersAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível carregar os usuários.", result.Message);
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenOperationIsCanceled_PropagatesCancellation()
    {
        _userRepository.ExceptionToThrowOnRead = new OperationCanceledException();

        await Assert.ThrowsAsync<OperationCanceledException>(() => _service.GetAllUsersAsync());
    }

    [Fact]
    public void GetUserById_WhenUserExists_ReturnsUserWithoutPassword()
    {
        User user = CreateExistingUser();

        var result = _service.GetUserById(user.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(new UserDto(user.Id, user.Name, user.Email, user.TypeUser, user.CreatedAt), result.Value);
    }

    [Fact]
    public void GetUserById_WhenUserDoesNotExistOrIsInactive_ReturnsNotFound()
    {
        var result = _service.GetUserById(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(UserNotFoundMessage, result.Message);
    }

    [Fact]
    public void GetUserById_WhenRepositoryThrows_ReturnsGenericFailure()
    {
        _userRepository.ExceptionToThrowOnRead = new InvalidOperationException(TechnicalErrorDetail);

        var result = _service.GetUserById(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível carregar o usuário.", result.Message);
    }

    [Fact]
    public void UpdateUser_WithOnlyName_ChangesNameAndKeepsOtherFields()
    {
        User user = CreateExistingUser();
        string originalEmail = user.Email;
        string originalPasswordHash = user.PasswordHash;

        var result = _service.UpdateUser(user.Id, new UpdateUserDto("Maria Silva", null, null));

        Assert.True(result.IsSuccess);
        Assert.Same(user, _userRepository.UpdatedUser);
        Assert.Equal("Maria Silva", user.Name);
        Assert.Equal(originalEmail, user.Email);
        Assert.Equal(originalPasswordHash, user.PasswordHash);
    }

    [Fact]
    public void UpdateUser_WithNewPassword_StoresHashInsteadOfPlainText()
    {
        User user = CreateExistingUser();
        const string newPassword = "NovaSenha@456";

        var result = _service.UpdateUser(user.Id, new UpdateUserDto(null, null, newPassword));

        Assert.True(result.IsSuccess);
        Assert.Equal(SecurityHelper.HashPassword(newPassword), user.PasswordHash);
        Assert.NotEqual(newPassword, user.PasswordHash);
    }

    [Fact]
    public void UpdateUser_WithNewEmail_ChangesEmail()
    {
        User user = CreateExistingUser();

        var result = _service.UpdateUser(user.Id, new UpdateUserDto(null, "maria.silva@email.com", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("maria.silva@email.com", user.Email);
    }

    [Fact]
    public void UpdateUser_WhenEmailBelongsToAnotherUser_ReturnsFailureWithoutUpdating()
    {
        User user = CreateExistingUser();
        _userRepository.EmailInUseByAnotherUser = true;

        var result = _service.UpdateUser(user.Id, new UpdateUserDto(null, "joao@email.com", null));

        Assert.False(result.IsSuccess);
        Assert.Equal(EmailInUseMessage, result.Message);
        Assert.Null(_userRepository.UpdatedUser);
        Assert.Equal(ValidUser.Email, user.Email);
    }

    [Fact]
    public void UpdateUser_WhenConcurrentUpdateViolatesUniqueEmail_ReturnsEmailInUse()
    {
        User user = CreateExistingUser();
        _userRepository.ExceptionToThrowOnUpdate =
            new DuplicateEntityException("Já existe um usuário com o e-mail informado.", new Exception("2601"));

        var result = _service.UpdateUser(user.Id, new UpdateUserDto(null, "joao@email.com", null));

        Assert.False(result.IsSuccess);
        Assert.Equal(EmailInUseMessage, result.Message);
    }

    [Fact]
    public void UpdateUser_WhenUserDoesNotExistOrIsInactive_ReturnsNotFound()
    {
        var result = _service.UpdateUser(Guid.NewGuid(), new UpdateUserDto("Maria", null, null));

        Assert.False(result.IsSuccess);
        Assert.Equal(UserNotFoundMessage, result.Message);
        Assert.Null(_userRepository.UpdatedUser);
    }

    [Fact]
    public void UpdateUser_WhenUnexpectedErrorOccurs_ReturnsGenericFailure()
    {
        User user = CreateExistingUser();
        _userRepository.ExceptionToThrowOnUpdate = new InvalidOperationException(TechnicalErrorDetail);

        var result = _service.UpdateUser(user.Id, new UpdateUserDto("Maria", null, null));

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível atualizar o usuário.", result.Message);
    }

    [Fact]
    public void UpdateUser_WhenOperationIsCanceled_PropagatesCancellation()
    {
        User user = CreateExistingUser();
        _userRepository.ExceptionToThrowOnUpdate = new OperationCanceledException();

        Assert.Throws<OperationCanceledException>(() => _service.UpdateUser(user.Id, new UpdateUserDto("Maria", null, null)));
    }

    [Fact]
    public void DeactivateUser_WhenUserIsActive_DeactivatesAndPersists()
    {
        User user = CreateExistingUser();

        var result = _service.DeactivateUser(user.Id);

        Assert.True(result.IsSuccess);
        Assert.False(user.IsActive);
        Assert.Same(user, _userRepository.UpdatedUser);
    }

    [Fact]
    public void DeactivateUser_WhenUserDoesNotExistOrIsInactive_ReturnsNotFound()
    {
        var result = _service.DeactivateUser(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(UserNotFoundMessage, result.Message);
        Assert.Null(_userRepository.UpdatedUser);
    }

    [Fact]
    public void DeactivateUser_WhenUnexpectedErrorOccurs_ReturnsGenericFailure()
    {
        User user = CreateExistingUser();
        _userRepository.ExceptionToThrowOnUpdate = new InvalidOperationException(TechnicalErrorDetail);

        var result = _service.DeactivateUser(user.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal("Não foi possível desativar o usuário.", result.Message);
    }

    private User CreateExistingUser()
    {
        var user = new User(ValidUser.Name, ValidUser.Email, SecurityHelper.HashPassword(ValidUser.Password));
        _userRepository.ActiveUser = user;
        return user;
    }
}
