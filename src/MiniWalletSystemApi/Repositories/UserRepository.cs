using Dapper;
using MiniWalletSystemApi.Entities;
using MiniWalletSystemApi.Infrastructure.DataAccess;
using MiniWalletSystemApi.Interfaces.Repositories;

namespace MiniWalletSystemApi.Repositories;

public class UserRepository: IUserRepository
{
    private readonly DapperExecutor _dapperExecutor;
    public UserRepository(DapperExecutor dapperExecutor)
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
                            (FullName, Email, RoleID, Notes)
                            VALUES
                            (@FullName, @Email, @RoleID, @Notes );
                            
                            SELECT CAST(SCOPE_IDENTITY() AS INT);
                            ";


        var result = await _dapperExecutor.ExecuteScalarAsync<int>(sql, userEntity);
        return result;  
    }
    
    
}