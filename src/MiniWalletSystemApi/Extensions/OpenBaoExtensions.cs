using MiniWalletSystemApi.Infrastructure.OpenBao;
using Polly;

namespace MiniWalletSystemApi.Extensions;

public static class OpenBaoExtensions
{
    public static async Task<WebApplicationBuilder> AddOpenBaoSecretsAsync(
        this WebApplicationBuilder builder)
    {

        // Console.WriteLine(
        //     $"[Startup] Environment: {builder.Environment.EnvironmentName}");
        //
        // Console.WriteLine(
        //     $"[Startup] OpenBao Address from config: " +
        //     $"'{builder.Configuration["OpenBao:Address"]}'");
        //
        // Console.WriteLine(
        //     $"[Startup] OpenBao Address from env: " +
        //     $"'{Environment.GetEnvironmentVariable("OpenBao__Address")}'");


        // --------------------------------------------------
        // Load OpenBao configuration
        // --------------------------------------------------

        var openBaoOptions = builder.Configuration
            .GetSection("OpenBao")
            .Get<OpenBaoOptions>()
            ?? throw new InvalidOperationException(
                "OpenBao configuration is not configured.");

        if (string.IsNullOrWhiteSpace(openBaoOptions.Address))
        {
            throw new InvalidOperationException(
                "OpenBao address is missing or empty.");
        }

        Console.WriteLine(
            $"[Startup] OpenBaoOptions.Address: " +
            $"'{openBaoOptions.Address}'");


        // --------------------------------------------------
        // Create OpenBao HTTP client
        // --------------------------------------------------

        using var openBaoHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler
                    .DangerousAcceptAnyServerCertificateValidator
        };

        using var openBaoHttpClient = new HttpClient(openBaoHandler)
        {
            BaseAddress = new Uri(openBaoOptions.Address)
        };


        // --------------------------------------------------
        // Create OpenBao service
        // --------------------------------------------------

        IOpenBaoService openBaoService =
            new OpenBaoService(
                openBaoHttpClient,
                openBaoOptions);


        // --------------------------------------------------
        // Load secrets with retry
        // --------------------------------------------------

        Dictionary<string, string> secrets;

        try
        {
            secrets = await Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: attempt =>
                        TimeSpan.FromSeconds(
                            Math.Pow(2, attempt)),
                    onRetry: (exception, delay, attempt, _) =>
                    {
                        Console.Error.WriteLine(
                            $"[Startup] OpenBao request failed " +
                            $"(attempt {attempt}/5): {exception.Message}. " +
                            $"Retrying in {delay.TotalSeconds}s...");
                    })
                .ExecuteAsync(
                    () => openBaoService.GetSecretsAsync());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(
                $"[Startup] FATAL: Unable to load secrets from OpenBao. " +
                $"API startup aborted: {ex.Message}");

            throw;
        }


        // --------------------------------------------------
        // Validate required secrets
        // --------------------------------------------------

        var jwtSecretKey = GetRequiredSecret(
            secrets,
            "JWT_KEY");

        var jwtIssuer = GetRequiredSecret(
            secrets,
            "JWT_ISSUER");

        var jwtAudience = GetRequiredSecret(
            secrets,
            "JWT_AUDIENCE");

        var dbConnectionString = GetRequiredSecret(
            secrets,
            "DB_CONNECTION_STRING");


        // --------------------------------------------------
        // Inject secrets into application configuration
        // --------------------------------------------------

        builder.Configuration["Jwt:SecretKey"] =
            jwtSecretKey;

        builder.Configuration["Jwt:Issuer"] =
            jwtIssuer;

        builder.Configuration["Jwt:Audience"] =
            jwtAudience;

        builder.Configuration["ConnectionStrings:DefaultConnection"] =
            dbConnectionString;


        Console.WriteLine(
            "[Startup] OpenBao secrets loaded successfully.");

        return builder;
    }


    private static string GetRequiredSecret(
        Dictionary<string, string> secrets,
        string key)
    {
        if (!secrets.TryGetValue(key, out var value) ||
            string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"OpenBao secret '{key}' is missing or empty.");
        }

        return value;
    }
}