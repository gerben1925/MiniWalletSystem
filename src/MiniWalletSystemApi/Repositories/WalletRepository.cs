using Dapper;
using MiniWalletSystemApi.Data;
using MiniWalletSystemApi.Models.Entities;
using MiniWalletSystemApi.Repositories.Interfaces;

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
            FROM [dbo].Wallet";

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