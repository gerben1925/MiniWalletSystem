namespace MiniWalletSystemApi.Interfaces.infrastructure.Configuration;

public interface IAppSettingProvider
{
    string GetString(string key);
    int GetInt(string key);
    bool GetBool(string key);
}