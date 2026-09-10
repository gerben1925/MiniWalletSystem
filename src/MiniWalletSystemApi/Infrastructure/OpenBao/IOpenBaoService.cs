namespace MiniWalletSystemApi.Infrastructure.OpenBao;

public interface IOpenBaoService
{
    Task<Dictionary<string, string>> GetSecretsAsync(
        CancellationToken cancellationToken = default);
}