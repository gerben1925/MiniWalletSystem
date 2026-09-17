using MiniWalletSystemApi.Interfaces.infrastructure.Common;

namespace MiniWalletSystemApi.Infrastructure.Common;

public class StringGenerator : IStringGenerator
{
    private readonly Random _random = new Random();
    
    public string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
    }
}