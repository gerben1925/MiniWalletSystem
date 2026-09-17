using Dapper;
using MiniWalletSystemApi.Interfaces.infrastructure.DataAccess;

namespace MiniWalletSystemApi.Infrastructure.DataAccess;

public class DapperExecutor
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DapperExecutor(IDbConnectionFactory dbConnectionFactory) 
    { 
        _dbConnectionFactory = dbConnectionFactory;
    }

    //  SELECT (multiple rows)
    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        return await connection.QueryAsync<T>(sql, param);
    }

    //  SELECT (single row)
    public async Task<T?> QuerySingleAsync<T>(string sql, object? param = null)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<T>(sql, param);
    }

    //  INSERT / UPDATE / DELETE
    public async Task<int> ExecuteAsync(string sql, object? param = null)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, param);
    }

    //  INSERT with return ID
    public async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<T>(sql, param);
    }

}