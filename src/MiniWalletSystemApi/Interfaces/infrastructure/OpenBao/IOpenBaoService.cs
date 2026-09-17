namespace MiniWalletSystemApi.Interfaces.infrastructure.OpenBao;

public interface IOpenBaoService
{
    Task<Dictionary<string, string>> GetSecretsAsync(
        CancellationToken cancellationToken = default);
}