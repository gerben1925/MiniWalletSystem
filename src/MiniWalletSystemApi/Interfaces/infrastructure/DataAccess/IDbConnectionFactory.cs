using System.Data;

namespace MiniWalletSystemApi.Interfaces.infrastructure.DataAccess
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
