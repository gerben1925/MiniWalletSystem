using MiniWalletSystemApi.Models.Entities;

namespace MiniWalletSystemApi.Repositories.Interfaces;

public interface IWalletRepository
{
    Task<IEnumerable<WalletEntity>> GetAll();
}