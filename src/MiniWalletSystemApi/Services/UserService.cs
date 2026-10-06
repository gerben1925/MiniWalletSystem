using MiniWalletSystemApi.Common;
using MiniWalletSystemApi.Entities;
using MiniWalletSystemApi.Interfaces.infrastructure.Common;
using MiniWalletSystemApi.Interfaces.infrastructure.Configuration;
using MiniWalletSystemApi.Interfaces.infrastructure.Email;
using MiniWalletSystemApi.Interfaces.Repositories;
using MiniWalletSystemApi.Interfaces.Services;
using MiniWalletSystemApi.Payloads.Requests;

namespace MiniWalletSystemApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAppSettingProvider _appSettingProvider;
    private readonly IStringGenerator  _stringGenerator;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplate _emailTemplate;
    private readonly IAccountVerificationRepository _accountVerificationRepository;
    
    public UserService(IUserRepository userRepository, IAppSettingProvider  appSettingProvider, IStringGenerator   stringGenerator, IEmailService  emailService, IEmailTemplate  emailTemplate,  IAccountVerificationRepository accountVerificationRepository)
    {
        _userRepository = userRepository;
        _appSettingProvider = appSettingProvider;
        _stringGenerator = stringGenerator;
        _emailService = emailService;
        _emailTemplate = emailTemplate;
        _accountVerificationRepository = accountVerificationRepository;
    }
    
    public async Task<int> RegisterAsync(NewUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (await _userRepository.IsAvailableUserByEmail(request.Email))
            return 0;
        
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
            CreatedAt = DateTime.UtcNow,
            Email = request.Email
        };
    }

    private static AccountVerificationEntity MapToNewAccountVerificationEntity(int userId, string token)
    {
        return new AccountVerificationEntity
        {
            UserId = userId,
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
        var emailContent = _emailTemplate.BuildVerificationEmailContent(user.Email, token, verificationLink);
        await _emailService.SendEmailAsync(user.Email, emailContent.emailSubject, emailContent.emailBody);
    }

    private async Task<int> SaveVerificationTokenAsync(int userId, string token)
    {
        var entity = MapToNewAccountVerificationEntity(userId, token);
        return await _accountVerificationRepository.NewAccountVerificationToken(entity);
    }


    
}