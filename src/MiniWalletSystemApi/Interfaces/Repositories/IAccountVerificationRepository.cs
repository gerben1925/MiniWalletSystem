using MiniWalletSystemApi.Entities;

namespace MiniWalletSystemApi.Interfaces.Repositories;

public interface IAccountVerificationRepository
{
    Task<int> NewAccountVerificationToken(AccountVerificationEntity accountVerificationEntity);
}