using MiniWalletSystemApi.Models.Entities;
using MiniWalletSystemApi.Models.Payloads.Responses;
using MiniWalletSystemApi.Repositories.Interfaces;
using MiniWalletSystemApi.Services.Interfaces;

namespace MiniWalletSystemApi.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository  _walletRepository;
    private readonly ILogger<WalletService> _logger;
    public  WalletService(IWalletRepository walletRepository, ILogger<WalletService>  logger)
    {
        _walletRepository = walletRepository;
        _logger = logger;
    }
    
    public async Task<IEnumerable<WalletResponse>> GetAll()
    {
        const string method = nameof(GetAll);
        _logger.LogInformation("{Method} started - Retrieving user overviews", method);
        
        try
        {
            var wallets = await _walletRepository.GetAll() ?? Enumerable.Empty<WalletEntity>(); 
           
            var walletList = wallets.ToList();
            if (walletList.Count == 0)
            {
                _logger.LogInformation("{Method} completed - No wallets found", method);
                return Enumerable.Empty<WalletResponse>();
            }

            var result = walletList.Select(MapToDto).ToList();

            _logger.LogInformation("{Method} succeeded - Retrieved {Count} wallets", method, result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Method} failed - Error retrieving wallets", method);
            throw new ApplicationException("Failed to retrieve wallets", ex);
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