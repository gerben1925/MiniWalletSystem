using MiniWalletSystemApi.Infrastructure.Common;
using MiniWalletSystemApi.Infrastructure.Configuration;
using MiniWalletSystemApi.Infrastructure.DataAccess;
using MiniWalletSystemApi.Infrastructure.Email;
using MiniWalletSystemApi.Infrastructure.Security;
using MiniWalletSystemApi.Interfaces.infrastructure.Common;
using MiniWalletSystemApi.Interfaces.infrastructure.Configuration;
using MiniWalletSystemApi.Interfaces.infrastructure.DataAccess;
using MiniWalletSystemApi.Interfaces.infrastructure.Email;
using MiniWalletSystemApi.Interfaces.infrastructure.Security;

namespace MiniWalletSystemApi.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices( this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IAppSettingProvider, AppSettingProvider>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTemplate,EmailTemplate>();
        services.AddScoped<IStringGenerator, StringGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        
        
        
        return services;
    }
}