using Dapper;
using FIAP.CloudGames.DataBase.Context;
using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.ReadModels;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FIAP.CloudGames.DataBase.Repositories;

public class LibraryRepository : ILibraryRepository
{
    private readonly AppDbContext _context;
    private readonly ISqlServerConnectionFactory _connectionFactory;
    public LibraryRepository(AppDbContext context, ISqlServerConnectionFactory sqlServerConnectionFactory)
    {
        _context = context;
        _connectionFactory = sqlServerConnectionFactory;
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
            g.Price AS PricePaid,
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
        var rows = await connection.QueryAsync<UserLibraryItemRow>(
            new CommandDefinition(
                sql,
                userId,
                cancellationToken: ct));

        return rows.AsList();
    }

}