using Microsoft.AspNetCore.Diagnostics;
using MiniWalletSystemApi.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

await builder.AddOpenBaoSecretsAsync();

builder.AddSerilogLogging();

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddJwtAuthentication(
    builder.Configuration);
builder.Services.AddServiceCollections();


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