using Dapper;
using FIAP.CloudGames.DataBase.Context;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Exceptions;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Infra.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.DataBase.Repositories;

public class UserRepository : IUserRepository
{
    private const int SqlServerUniqueIndexViolation = 2601;
    private const int SqlServerUniqueConstraintViolation = 2627;

    private readonly AppDbContext _context;
    private readonly ISqlServerConnectionFactory _connectionFactory;
    private readonly ILogger<UserRepository> _logger;
    public UserRepository(AppDbContext context, ISqlServerConnectionFactory sqlServerConnectionFactory, ILogger<UserRepository> logger)
    {
        _context = context;
        _connectionFactory = sqlServerConnectionFactory;
        _logger = logger;
    }

    public void CreateUser(User user)
    {
        _context.Users.Add(user);
        SaveChangesTranslatingDuplicateEmail();
    }

    public void UpdateUser(User user)
    {
        _context.Users.Update(user);
        SaveChangesTranslatingDuplicateEmail();
    }

    public User? FindLogin(string email, string password)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password && u.IsActive);
    }

    public bool VerifyIfExist(string email)
    {
        return _context.Users.Any(u => u.Email == email);
    }

    public bool VerifyIfExist(string email, Guid ignoredUserId)
    {
        return _context.Users.Any(u => u.Email == email && u.Id != ignoredUserId);
    }

    public User? GetActiveUserById(Guid id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id && u.IsActive);
    }

    public async Task<IReadOnlyList<UserListRow>> GetAllActiveUsersAsync(CancellationToken ct = default)
    {
        const string sql = """
        SELECT
            Id,
            Name,
            Email,
            TypeUser,
            CreatedAt
        FROM Users
        WHERE IsActive = 1
        ORDER BY Name;
        """;

        using IDbConnection connection = _connectionFactory.CreateConnection();
        long startTimestamp = Stopwatch.GetTimestamp();

        IEnumerable<UserListRow> users = await connection.QueryAsync<UserListRow>(
            new CommandDefinition(sql, cancellationToken: ct));

        List<UserListRow> activeUsers = users.AsList();
        LogDapperCommand(nameof(GetAllActiveUsersAsync), sql, activeUsers.Count, startTimestamp);

        return activeUsers;
    }

    private void SaveChangesTranslatingDuplicateEmail()
    {
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException ex) when (IsUniqueKeyViolation(ex))
        {
            throw new DuplicateEntityException("Já existe um usuário com o e-mail informado.", ex);
        }
    }

    private static bool IsUniqueKeyViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: SqlServerUniqueIndexViolation or SqlServerUniqueConstraintViolation };

    private void LogDapperCommand(string operation, string sql, int rowCount, long startTimestamp)
    {
        _logger.LogInformation("Dapper executou {Operation} em {ElapsedMilliseconds} ms ({RowCount} linhas). SQL: {Sql}",
            operation,
            Math.Round(Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds, 2),
            rowCount,
            sql);
    }
}
