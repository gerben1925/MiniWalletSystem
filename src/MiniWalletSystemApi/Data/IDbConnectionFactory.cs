using System.Data;

namespace MiniWalletSystemApi.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
