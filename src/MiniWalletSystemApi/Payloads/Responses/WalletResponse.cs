namespace MiniWalletSystemApi.Payloads.Responses;

public class WalletResponse
{
    public int  Id { get; set; }
    public string? UsersId { get; set; } 
    public long AccountNumber  { get; set; }
    public  decimal Balance { get; set; }
    public DateTime DaTeCreated { get; set; }
}