using MiniWalletSystemApi.Models.Payloads.Responses;

namespace MiniWalletSystemApi.Services.Interfaces;

public interface IWalletService
{
    Task<IEnumerable<WalletResponse>> GetAll();
}