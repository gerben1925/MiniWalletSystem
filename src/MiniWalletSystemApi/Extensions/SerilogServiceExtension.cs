using Serilog;
using Serilog.Events;

namespace MiniWalletSystemApi.Extensions;

public static class SerilogServiceExtension
{
    public static void AddSerilogLogging(this WebApplicationBuilder builder)
    {
        var logPath = Path.Combine(
            AppContext.BaseDirectory,
            "Logs",
            "log-.txt");

        var errorLogPath = Path.Combine(
            AppContext.BaseDirectory,
            "Logs",
            "errors-.txt");

        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()

                // Console
                .WriteTo.Console()

                // All logs
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day)

                // Error and Fatal only
                .WriteTo.File(
                    errorLogPath,
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    rollingInterval: RollingInterval.Day);
        });
    }
}