using MiniWalletSystemApi.Common;
using MiniWalletSystemApi.Entities;
using MiniWalletSystemApi.Interfaces.infrastructure.Common;
using MiniWalletSystemApi.Interfaces.infrastructure.Configuration;
using MiniWalletSystemApi.Interfaces.Repositories;
using MiniWalletSystemApi.Interfaces.Services;
using MiniWalletSystemApi.Payloads.Requests;

namespace MiniWalletSystemApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAppSettingProvider _appSettingProvider;
    private readonly IStringGenerator  _stringGenerator;
    
    public UserService(IUserRepository userRepository, IAppSettingProvider  appSettingProvider, IStringGenerator   stringGenerator)
    {
        _userRepository = userRepository;
        _appSettingProvider = appSettingProvider;
        _stringGenerator = stringGenerator;
    }
    
    public async Task<int> RegisterAsync(NewUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = MapToUserEntity(request);
        int userId = await _userRepository.RegesterNewUser(user);

        if (userId <= 0) return 0;

        var token = GenerateVerificationToken();
        var verificationLink = BuildVerificationLink(token);
        await SendVerificationEmailAsync(user, token, verificationLink);
        return await SaveVerificationTokenAsync(userId, token);
    }

    private static UserEntity MapToUserEntity(NewUserRequest request)
    {
        return new UserEntity
        {
            FullName = request.FullName,
            CreatedAt = DateTime.UtcNow,
            Email = request.Email,
            RoleID = request.RoleID,
            Notes = request.Notes
        };
    }

    private static AccountVerificationEntity MapToNewAccountVerificationEntity(int userId, string token)
    {
        return new AccountVerificationEntity
        {
            UserID = userId,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false
        };
    }

    private string GenerateVerificationToken()
    {
        var tokenLength = _appSettingProvider.GetInt("EmailVerificationAccountSettings:TokenLength");
        return _stringGenerator.GenerateRandomString(tokenLength);
    }

    private string BuildVerificationLink(string token)
    {
        var baseUrl = _appSettingProvider.GetString("FrontEnd:BaseUrl");
        return $"{baseUrl.TrimEnd('/')}/verify-account?token={token}";
    }

    private async Task SendVerificationEmailAsync(UserEntity user, string token, string verificationLink)
    {
        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        var emailContent = _emailTemplateService.BuildVerificationEmailContent(fullName, user.Email, token, verificationLink);
        await _emailService.SendEmailAsync(user.Email, emailContent.emailSubject, emailContent.emailBody);
    }

    private async Task<int> SaveVerificationTokenAsync(int userId, string token)
    {
        var entity = MapToNewAccountVerificationEntity(userId, token);
        return await _accountVerificationRepository.NewAccountVerificationToken(entity);
    }


    
}