using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace FIAP.CloudGames.Infra.Data;

public class SqlServerConnectionFactory : ISqlServerConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Conexão não encontrada");
    }
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
