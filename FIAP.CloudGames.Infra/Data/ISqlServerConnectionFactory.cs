using System.Data;

namespace FIAP.CloudGames.Infra.Data;

public interface ISqlServerConnectionFactory
{
    IDbConnection CreateConnection();
}