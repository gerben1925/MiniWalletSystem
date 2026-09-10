
using Microsoft.AspNetCore.Diagnostics;
using MiniWalletSystemApi.Data;
using MiniWalletSystemApi.Extensions;
using MiniWalletSystemApi.Repositories;
using MiniWalletSystemApi.Repositories.Interfaces;
using MiniWalletSystemApi.Services;
using MiniWalletSystemApi.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


await builder.AddOpenBaoSecretsAsync();


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
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
            rollingInterval: RollingInterval.Day);
});


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
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        Log.Error(ex, "Unhandled exception on {Path}", context.Request.Path);

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(
            new { error = "An unexpected error occurred." });
    });
});


app.UseAuthorization();

app.MapControllers();

app.Run();