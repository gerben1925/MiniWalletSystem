namespace MiniWalletSystemApi.Interfaces.infrastructure.Security;

public interface IPasswordHasher
{
    (string hash, byte[] salt) HashPassword(string password);
    bool VerifyPassword(string password, string storedHash, byte[] storedSalt);
}