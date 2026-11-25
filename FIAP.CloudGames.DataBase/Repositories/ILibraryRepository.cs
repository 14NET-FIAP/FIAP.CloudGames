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
using System.Data;
using System.Diagnostics;

namespace FIAP.CloudGames.DataBase.Repositories;

public class LibraryRepository : ILibraryRepository
{
    private const int SqlServerPrimaryKeyViolation = 2627;

    private readonly AppDbContext _context;
    private readonly ISqlServerConnectionFactory _connectionFactory;
    private readonly ILogger<LibraryRepository> _logger;
    public LibraryRepository(AppDbContext context, ISqlServerConnectionFactory sqlServerConnectionFactory, ILogger<LibraryRepository> logger)
    {
        _context = context;
        _connectionFactory = sqlServerConnectionFactory;
        _logger = logger;
    }


    public async Task<IReadOnlyList<UserLibraryItemRow>> GetUserLibraryAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        const string sql = """
        SELECT
            ul.GameId,
            g.Title,
            g.Description,
            ul.PricePaid,
            g.ReleaseDate,
            ul.AcquisitionDate
        FROM UserLibrary ul
        INNER JOIN Game g
            ON g.Id = ul.GameId
        WHERE ul.UserId = @UserId
          AND g.IsActive = 1
        ORDER BY ul.AcquisitionDate DESC;
        """;

        using IDbConnection connection = _connectionFactory.CreateConnection();
        long startTimestamp = Stopwatch.GetTimestamp();

        var rows = await connection.QueryAsync<UserLibraryItemRow>(
            new CommandDefinition(
                sql,
                new { UserId = userId },
                cancellationToken: ct));

        List<UserLibraryItemRow> libraryRows = rows.AsList();
        LogDapperCommand(nameof(GetUserLibraryAsync), sql, libraryRows.Count, startTimestamp);

        return libraryRows;
    }

    public void AddGameToLibrary(UserLibrary userLibrary)
    {
        _context.UserLibrary.Add(userLibrary);

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException ex) when (IsPrimaryKeyViolation(ex))
        {
            throw new DuplicateEntityException("O jogo já está na biblioteca do usuário.", ex);
        }
    }

    public bool RemoveGameFromLibrary(Guid userId, Guid gameId)
    {
        int removedRows = _context.UserLibrary
            .Where(ul => ul.UserId == userId && ul.GameId == gameId)
            .ExecuteDelete();

        return removedRows > 0;
    }

    public bool VerifyIfExist(Guid userId, Guid gameId) => _context.UserLibrary.Any(ul => ul.UserId == userId && ul.GameId == gameId);

    private static bool IsPrimaryKeyViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: SqlServerPrimaryKeyViolation };

    private void LogDapperCommand(string operation, string sql, int rowCount, long startTimestamp)
    {
        _logger.LogInformation("Dapper executou {Operation} em {ElapsedMilliseconds} ms ({RowCount} linhas). SQL: {Sql}",
            operation,
            Math.Round(Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds, 2),
            rowCount,
            sql);
    }
}
