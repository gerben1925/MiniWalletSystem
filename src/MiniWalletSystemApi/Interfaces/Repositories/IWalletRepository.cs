using MiniWalletSystemApi.Entities;

namespace MiniWalletSystemApi.Interfaces.Repositories;

public interface IWalletRepository
{
    Task<IEnumerable<WalletEntity>> GetAll();
}