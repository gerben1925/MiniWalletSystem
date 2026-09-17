namespace MiniWalletSystemApi.Payloads.Requests;

public class NewUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleID { get; set; }
    public string Notes { get; set; } = string.Empty;
}