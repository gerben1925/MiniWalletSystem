
namespace MiniWalletSystemApi.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceCollections(this IServiceCollection services)
    {
        
        services.AddInfrastructureServices();
        services.AddApplicationServices();
        services.AddRepositoryServices();

        

        return services;
    }

}