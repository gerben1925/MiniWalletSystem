using System.Text;
using System.Text.Json;


namespace MiniWalletSystemApi.Infrastructure.OpenBao;

public sealed class OpenBaoService : IOpenBaoService
{
    private readonly HttpClient _httpClient;
    private readonly OpenBaoOptions _options;

    public OpenBaoService(
        HttpClient httpClient,
        OpenBaoOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<Dictionary<string, string>> GetSecretsAsync(
        CancellationToken cancellationToken = default)
    {
        var token = await LoginAsync(cancellationToken);

        return await FetchSecretsAsync(
            token,
            cancellationToken);
    }

    // --------------------------------------------------
    // Authenticate using OpenBao AppRole
    // --------------------------------------------------

    private async Task<string> LoginAsync(
        CancellationToken cancellationToken)
    {
        var loginPayload = new
        {
            role_id = _options.RoleId,
            secret_id = _options.SecretId
        };

        using var loginContent = new StringContent(
            JsonSerializer.Serialize(loginPayload),
            Encoding.UTF8,
            "application/json");

        using var loginResponse = await _httpClient.PostAsync(
            "/v1/auth/approle/login",
            loginContent,
            cancellationToken);

        if (!loginResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"OpenBao AppRole login failed with status " +
                $"{(int)loginResponse.StatusCode} " +
                $"({loginResponse.StatusCode}).");
        }

        var loginResult = await loginResponse.Content
            .ReadFromJsonAsync<JsonElement>(
                cancellationToken: cancellationToken);

        if (!loginResult.TryGetProperty(
                "auth",
                out var auth))
        {
            throw new InvalidOperationException(
                "OpenBao login response did not contain 'auth'.");
        }

        if (!auth.TryGetProperty(
                "client_token",
                out var tokenElement))
        {
            throw new InvalidOperationException(
                "OpenBao login response did not contain " +
                "'auth.client_token'.");
        }

        var token = tokenElement.GetString();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                "OpenBao returned an empty authentication token.");
        }

        return token;
    }

    // --------------------------------------------------
    // Fetch KV v2 secret
    // --------------------------------------------------

    private async Task<Dictionary<string, string>> FetchSecretsAsync(
        string token,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/v1/{_options.SecretPath}");

        request.Headers.Add(
            "X-Vault-Token",
            token);

        using var secretResponse = await _httpClient.SendAsync(
            request,
            cancellationToken);

        if (!secretResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"OpenBao secret fetch failed for path " +
                $"'{_options.SecretPath}' with status " +
                $"{(int)secretResponse.StatusCode} " +
                $"({secretResponse.StatusCode}).");
        }

        var secretResult = await secretResponse.Content
            .ReadFromJsonAsync<JsonElement>(
                cancellationToken: cancellationToken);


        if (!secretResult.TryGetProperty(
                "data",
                out var outerData))
        {
            throw new InvalidOperationException(
                $"Unexpected OpenBao response shape for " +
                $"path '{_options.SecretPath}'. " +
                "Expected KV v2 response.");
        }

        if (!outerData.TryGetProperty(
                "data",
                out var data))
        {
            throw new InvalidOperationException(
                $"Unexpected OpenBao response shape for " +
                $"path '{_options.SecretPath}'. " +
                "Expected KV v2 'data.data'.");
        }

        var secrets = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var property in data.EnumerateObject())
        {
            secrets[property.Name] =
                property.Value.ValueKind switch
                {
                    JsonValueKind.String =>
                        property.Value.GetString() ?? string.Empty,

                    JsonValueKind.Null =>
                        string.Empty,

                    _ =>
                        property.Value.GetRawText()
                };
        }

        // --------------------------------------------------
        // Revoke bootstrap token
        // --------------------------------------------------


        try
        {
            using var revokeRequest =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "/v1/auth/token/revoke-self");

            revokeRequest.Headers.Add(
                "X-Vault-Token",
                token);

            await _httpClient.SendAsync(
                revokeRequest,
                cancellationToken);
        }
        catch
        {
            // Non-fatal.
            //
            // If revocation fails, OpenBao will eventually
            // expire the token according to its TTL.
        }

        return secrets;
    }
}