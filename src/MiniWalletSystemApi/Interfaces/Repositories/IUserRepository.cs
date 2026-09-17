using MiniWalletSystemApi.Entities;

namespace MiniWalletSystemApi.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserEntity> GetUserByName(string username);
    Task<int> RegesterNewUser(UserEntity userEntity);
}