using MiniWalletSystemApi.Entities;
using MiniWalletSystemApi.Infrastructure.DataAccess;
using MiniWalletSystemApi.Interfaces.infrastructure.DataAccess;
using MiniWalletSystemApi.Interfaces.Repositories;

namespace MiniWalletSystemApi.Repositories;

public class AccountVerificationRepository: IAccountVerificationRepository
{
    private readonly IDapperExecutor  _dapperExecutor;

    public AccountVerificationRepository(IDapperExecutor  dapperExecutor)
    {
        _dapperExecutor = dapperExecutor;
    }
    
    public async Task<int> NewAccountVerificationToken(AccountVerificationEntity accountVerificationEntity)
    {
        const string sql = @"
                              
                            INSERT INTO AccountVerification 
                            (UserId, Token, CreatedAt, IsUsed)
                            VALUES
                            (@UserId,@Token, @CreatedAt,@IsUsed);
                            
                            SELECT CAST(SCOPE_IDENTITY() AS INT);
                            ";


        var result = await _dapperExecutor.ExecuteScalarAsync<int>(sql, accountVerificationEntity);
        return result;
    }
    
}