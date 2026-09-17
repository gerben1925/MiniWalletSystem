using MiniWalletSystemApi.Interfaces.Repositories;
using MiniWalletSystemApi.Repositories;

namespace MiniWalletSystemApi.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAccountVerificationRepository, AccountVerificationRepository>();

        return services;
    }
}