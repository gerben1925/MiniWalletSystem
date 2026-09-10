
using MiniWalletSystemApi.Data;
using MiniWalletSystemApi.Extensions;
using MiniWalletSystemApi.Repositories;
using MiniWalletSystemApi.Repositories.Interfaces;
using MiniWalletSystemApi.Services;
using MiniWalletSystemApi.Services.Interfaces;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// Diagnostic: OpenBao configuration
// --------------------------------------------------

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
//
// Docker Compose:
//
// OpenBao__Address
// OpenBao__RoleId
// OpenBao__SecretId
// OpenBao__SecretPath
//
// ASP.NET Core maps these to:
//
// OpenBao:Address
// OpenBao:RoleId
// OpenBao:SecretId
// OpenBao:SecretPath
//

// var openBaoOptions = builder.Configuration
//     .GetSection("OpenBao")
//     .Get<OpenBaoOptions>()
//     ?? throw new InvalidOperationException(
//         "OpenBao configuration is not configured.");
//
// Console.WriteLine(
//     $"[Startup] OpenBaoOptions.Address: " +
//     $"'{openBaoOptions.Address}'");


// --------------------------------------------------
// Load secrets from OpenBao
// --------------------------------------------------
//
// This happens before builder.Build(), therefore the
// application's DI container is not available yet.
//
// We manually create OpenBaoService here because the
// application needs the secrets before it can finish
// building the application.
//

// using var openBaoHandler = new HttpClientHandler
// {
//     ServerCertificateCustomValidationCallback =
//         HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
// };
//
// using (var openBaoHttpClient = new HttpClient(openBaoHandler)
// {
//     BaseAddress = new Uri(openBaoOptions.Address)
// })
// {
//     IOpenBaoService openBaoService =
//         new OpenBaoService(
//             openBaoHttpClient,
//             openBaoOptions);
//
//     Dictionary<string, string> secrets;
//
//     try
//     {
//         secrets = await Polly.Policy
//             .Handle<HttpRequestException>()
//             .WaitAndRetryAsync(
//                 retryCount: 5,
//                 sleepDurationProvider: attempt =>
//                     TimeSpan.FromSeconds(
//                         Math.Pow(2, attempt)),
//                 onRetry: (exception, delay, attempt, _) =>
//                 {
//                     Console.Error.WriteLine(
//                         $"[Startup] OpenBao request failed " +
//                         $"(attempt {attempt}/5): {exception.Message}. " +
//                         $"Retrying in {delay.TotalSeconds}s...");
//                 })
//             .ExecuteAsync(
//                 () => openBaoService.GetSecretsAsync());
//     }
//     catch (Exception ex)
//     {
//         Console.Error.WriteLine(
//             $"[Startup] FATAL: Unable to load secrets from OpenBao. " +
//             $"API startup aborted: {ex.Message}");
//
//         throw;
//     }
//
//
//
//
//     if (!secrets.TryGetValue(
//             "JWT_KEY",
//             out var jwtSecretKey) ||
//         string.IsNullOrWhiteSpace(jwtSecretKey))
//     {
//         throw new InvalidOperationException(
//             "OpenBao secret 'JWT_KEY' is missing or empty.");
//     }
//
//     if (!secrets.TryGetValue(
//             "JWT_ISSUER",
//             out var jwtIssuer) ||
//         string.IsNullOrWhiteSpace(jwtIssuer))
//     {
//         throw new InvalidOperationException(
//             "OpenBao secret 'JWT_ISSUER' is missing or empty.");
//     }
//
//     if (!secrets.TryGetValue(
//             "JWT_AUDIENCE",
//             out var jwtAudience) ||
//         string.IsNullOrWhiteSpace(jwtAudience))
//     {
//         throw new InvalidOperationException(
//             "OpenBao secret 'JWT_AUDIENCE' is missing or empty.");
//     }
//
//     if (!secrets.TryGetValue(
//             "DB_CONNECTION_STRING",
//             out var dbConnectionString) ||
//         string.IsNullOrWhiteSpace(dbConnectionString))
//     {
//         throw new InvalidOperationException(
//             "OpenBao secret 'DB_CONNECTION_STRING' is missing or empty.");
//     }
//
//
//
//     builder.Configuration["Jwt:SecretKey"] =
//         jwtSecretKey;
//
//     builder.Configuration["Jwt:Issuer"] =
//         jwtIssuer;
//
//     builder.Configuration["Jwt:Audience"] =
//         jwtAudience;
//
//     builder.Configuration["ConnectionStrings:DefaultConnection"] =
//         dbConnectionString;
// }


await builder.AddOpenBaoSecretsAsync();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddScoped<IDbConnectionFactory,
    DbConnectionFactory>();

builder.Services.AddScoped<IWalletRepository,
    WalletRepository>();

builder.Services.AddScoped<IWalletService,
    WalletService>();

builder.Services.AddJwtAuthentication(
    builder.Configuration);



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();