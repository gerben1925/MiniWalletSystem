using Microsoft.Data.SqlClient;
using System.Data;
using MiniWalletSystemApi.Interfaces.infrastructure;
using MiniWalletSystemApi.Interfaces.infrastructure.DataAccess;


namespace MiniWalletSystemApi.Infrastructure.DataAccess
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection")
                                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            return new SqlConnection(connectionString);
        }
    }
}
