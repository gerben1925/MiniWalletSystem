using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
using MiniWalletSystemApi.Interfaces.infrastructure;
using MiniWalletSystemApi.Interfaces.infrastructure.Security;


namespace MiniWalletSystemApi.Infrastructure.Security;

public class PasswordHasher: IPasswordHasher
{
    public (string hash, byte[] salt) HashPassword(string password)
    {
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = saltBytes,
            DegreeOfParallelism = 8,
            MemorySize = 65536,
            Iterations = 4
        };

        var hashBytes = argon2.GetBytes(32);
        return (Convert.ToBase64String(hashBytes), saltBytes);
    }

    public bool VerifyPassword(string password, string storedHash, byte[] storedSalt)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = storedSalt,
            DegreeOfParallelism = 8,
            MemorySize = 65536,
            Iterations = 4
        };

        var computedHash = argon2.GetBytes(32);
        return Convert.ToBase64String(computedHash) == storedHash;
    }
}