using FIAP.CloudGames.Application.DTOs.User;
using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Mappings;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Exceptions;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Domain.Services;
using FIAP.CloudGames.Infra.Security;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Application.Services;

public class UsersService : IUsersService
{
    private const string UserAlreadyExistsMessage = "Usuário já cadastrado no sistema";
    private const string EmailInUseMessage = "O e-mail informado já está em uso por outro usuário.";
    private const string UserNotFoundMessage = "Usuário não encontrado.";

    private readonly IUserRepository _usersRepository;
    private readonly ILogger<UsersService> _logger;
    public UsersService(IUserRepository usersRepository, ILogger<UsersService> logger)
    {
        _usersRepository = usersRepository;
        _logger = logger;
    }


    public Result CreateUser(CreateUserDto userDto)
    {
        try
        {
            User user = UserMapping.MapUserToCreate(userDto);

            if (VerifyIfExist(user.Email))
                return Result.Failure(UserAlreadyExistsMessage);

            _usersRepository.CreateUser(user);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (DuplicateEntityException)
        {
            // Outra requisição gravou o mesmo e-mail entre a verificação e o insert.
            _logger.LogWarning("Cadastro de usuário rejeitado pelo índice único de e-mail (cadastro concorrente)");
            return Result.Failure(UserAlreadyExistsMessage);
        }
    }

    public async Task<ResultList<UserDto>> GetAllUsersAsync(CancellationToken ct = default)
    {
        IReadOnlyList<UserListRow> users = await _usersRepository.GetAllActiveUsersAsync(ct);

        var dtos = new List<UserDto>(users.Count);
        foreach (var user in users)
            dtos.Add(user.MapUserRowToDto());

        return ResultList<UserDto>.Success(dtos);
    }

    public Result<UserDto> GetUserById(Guid id)
    {
        User? user = _usersRepository.GetActiveUserById(id);
        if (user is null)
            return Result<UserDto>.Failure(UserNotFoundMessage);

        return Result<UserDto>.Success(user.MapUserToDto());
    }

    public Result UpdateUser(Guid id, UpdateUserDto userDto)
    {
        try
        {
            User? user = _usersRepository.GetActiveUserById(id);
            if (user is null)
                return Result.Failure(UserNotFoundMessage);

            if (IsEmailInUseByAnotherUser(userDto.Email, id))
                return Result.Failure(EmailInUseMessage);

            string? passwordHash = userDto.Password is null ? null : SecurityHelper.HashPassword(userDto.Password);
            user.Update(userDto.Name, userDto.Email, passwordHash);
            _usersRepository.UpdateUser(user);

            _logger.LogInformation("Usuário {UserId} atualizado", id);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (DuplicateEntityException)
        {
            _logger.LogWarning("Atualização do usuário {UserId} rejeitada pelo índice único de e-mail (alteração concorrente)", id);
            return Result.Failure(EmailInUseMessage);
        }
    }

    public Result DeactivateUser(Guid id)
    {
        User? user = _usersRepository.GetActiveUserById(id);
        if (user is null)
            return Result.Failure(UserNotFoundMessage);

        user.Deactivate();
        _usersRepository.UpdateUser(user);

        _logger.LogInformation("Usuário {UserId} desativado", id);
        return Result.Success();
    }

    private bool VerifyIfExist(string email) => _usersRepository.VerifyIfExist(email);

    private bool IsEmailInUseByAnotherUser(string? email, Guid userId) =>
        !string.IsNullOrWhiteSpace(email) && _usersRepository.VerifyIfExist(email, userId);
}
