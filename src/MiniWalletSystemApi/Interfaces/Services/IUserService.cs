using MiniWalletSystemApi.Payloads.Requests;

namespace MiniWalletSystemApi.Interfaces.Services;

public interface IUserService
{
    Task<int> RegisterAsync(NewUserRequest request);
}