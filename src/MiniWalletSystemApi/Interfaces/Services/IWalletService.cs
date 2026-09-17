using MiniWalletSystemApi.Payloads.Responses;

namespace MiniWalletSystemApi.Interfaces.Services;

public interface IWalletService
{
    Task<IEnumerable<WalletResponse>> GetAll();
}