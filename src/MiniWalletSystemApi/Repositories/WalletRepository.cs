using Dapper;
using MiniWalletSystemApi.Entities;
using MiniWalletSystemApi.Interfaces.infrastructure.DataAccess;
using MiniWalletSystemApi.Interfaces.Repositories;

namespace MiniWalletSystemApi.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    
    public WalletRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    
    
    public async Task<IEnumerable<WalletEntity>> GetAll()
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        connection.Open();

        try
        {
            const string sqlQuery = @"
            SELECT *
            FROM [dbo].Wallets";

            var wallets = await connection.QueryAsync<WalletEntity>(sqlQuery);
            return wallets;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving Wallets: {ex.Message}");
            throw;
        }
    }
    
}