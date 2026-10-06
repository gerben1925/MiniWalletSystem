namespace MiniWalletSystemApi.Interfaces.infrastructure.Common;

public interface IStringGenerator
{
    string GenerateRandomString(int length);
}