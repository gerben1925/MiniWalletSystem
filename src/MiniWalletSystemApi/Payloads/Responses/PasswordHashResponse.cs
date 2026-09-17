namespace MiniWalletSystemApi.Payloads.Responses;

public class PasswordHashResponse
{
    public string PasswordHash { get; set; } = string.Empty;
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

}