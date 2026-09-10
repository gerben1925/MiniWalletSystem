using MiniWalletSystemApi.Models.Entities;
using MiniWalletSystemApi.Models.Payloads.Responses;
using MiniWalletSystemApi.Repositories.Interfaces;
using MiniWalletSystemApi.Services.Interfaces;

namespace MiniWalletSystemApi.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository  _walletRepository;
    public  WalletService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }
    
    public async Task<IEnumerable<WalletResponse>> GetAll()
    {
        try
        {
            var wallets = await _walletRepository.GetAll();
            if (wallets == null || !wallets.Any())
            {
                return Enumerable.Empty<WalletResponse>();
            }
            return wallets.Select(MapToDto);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    private static WalletResponse MapToDto(WalletEntity entity)
    {
        return new WalletResponse
        {
            Id = entity.Id,
            UsersId = entity.UsersId,
            AccountNumber = entity.AccountNumber,
            Balance = entity.Balance,
            DaTeCreated = entity.DaTeCreated
        };
    }
    
    
}