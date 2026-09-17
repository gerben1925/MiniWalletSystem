using MiniWalletSystemApi.Interfaces.Services;
using MiniWalletSystemApi.Services;

namespace MiniWalletSystemApi.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAccountVerificationService, AccountVerificationService>();


        return services;
    }
}