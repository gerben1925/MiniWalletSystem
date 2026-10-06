using Dapper;
using MiniWalletSystemApi.Entities;
using MiniWalletSystemApi.Infrastructure.DataAccess;
using MiniWalletSystemApi.Interfaces.infrastructure.DataAccess;
using MiniWalletSystemApi.Interfaces.Repositories;

namespace MiniWalletSystemApi.Repositories;

public class UserRepository: IUserRepository
{
    private readonly IDapperExecutor _dapperExecutor;
    public UserRepository(IDapperExecutor dapperExecutor)
    {
        _dapperExecutor = dapperExecutor;
    }
    public async Task<UserEntity> GetUserByName(string username)
    {
        const string sqlQuery = @"
            SELECT *
            FROM [dbo].Users WHERE UserName = @username";

        var result = await _dapperExecutor.QuerySingleAsync<UserEntity>(sqlQuery, new { username = username });
        return result;
    }
    
    public async Task<int> RegesterNewUser(UserEntity userEntity)
    {
        const string sql = @"
                              
                            INSERT INTO Users 
                            (Email, CreatedAt)
                            VALUES
                            (@Email, @CreatedAt);
                            
                            SELECT CAST(SCOPE_IDENTITY() AS INT);
                            ";


        var result = await _dapperExecutor.ExecuteScalarAsync<int>(sql, userEntity);
        return result;  
    }
    
    public async Task<bool> IsAvailableUserByEmail(string email)
    {
        const string sqlQuery = @"
            SELECT *
            FROM [dbo].Users WHERE email = @Email";

        var result = await _dapperExecutor.QuerySingleAsync<bool>(sqlQuery, new { Email = email });
        if (result)
        {
            return  true;
        }
        return false;
    }
    
    
    
}