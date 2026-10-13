using MiniWalletSystemApi.Entities;
using MiniWalletSystemApi.Interfaces.infrastructure.Common;
using MiniWalletSystemApi.Interfaces.infrastructure.Configuration;
using MiniWalletSystemApi.Interfaces.infrastructure.Email;
using MiniWalletSystemApi.Interfaces.Repositories;
using MiniWalletSystemApi.Payloads.Requests;
using MiniWalletSystemApi.Services;
using Moq;
using Xunit;

namespace MiniWalletSystem.UnitTest.Services;

public class UserServiceTests
{
    
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IAppSettingProvider> _appSettingProviderMock = new();
    private readonly Mock<IStringGenerator> _stringGeneratorMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<IEmailTemplate> _emailTemplateMock = new();
    private readonly Mock<IAccountVerificationRepository>
        _accountVerificationRepositoryMock = new();

    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(
            _userRepositoryMock.Object,
            _appSettingProviderMock.Object,
            _stringGeneratorMock.Object,
            _emailServiceMock.Object,
            _emailTemplateMock.Object,
            _accountVerificationRepositoryMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldRegisterUserAndSaveVerificationToken()
    {
        // Arrange
        var request = new NewUserRequest
        {
            Email = "test@example.com"
        };

        const int userId = 123;
        const int tokenLength = 32;
        const string token = "ABC123XYZ456";
        const string baseUrl = "https://mywallet.com/";
        const string verificationLink =
            "https://mywallet.com/verify-account?token=ABC123XYZ456";
        const string subject = "Verify your account";
        const string body = "Please verify your account.";

        _userRepositoryMock
            .Setup(x => x.IsAvailableUserByEmail(request.Email))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.RegesterNewUser(It.IsAny<UserEntity>()))
            .ReturnsAsync(userId);

        _appSettingProviderMock
            .Setup(x => x.GetInt(
                "EmailVerificationAccountSettings:TokenLength"))
            .Returns(tokenLength);

        _stringGeneratorMock
            .Setup(x => x.GenerateRandomString(tokenLength))
            .Returns(token);

        _appSettingProviderMock
            .Setup(x => x.GetString("FrontEnd:BaseUrl"))
            .Returns(baseUrl);

        _emailTemplateMock
            .Setup(x => x.BuildVerificationEmailContent(
                request.Email, token, verificationLink))
            .Returns((subject, body));

        _emailServiceMock
            .Setup(x => x.SendEmailAsync(
                request.Email, subject, body))
            .Returns(Task.CompletedTask);

        _accountVerificationRepositoryMock
            .Setup(x => x.NewAccountVerificationToken(
                It.IsAny<AccountVerificationEntity>()))
            .ReturnsAsync(456);

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.Equal(456, result);

        _userRepositoryMock.Verify(
            x => x.RegesterNewUser(
                It.Is<UserEntity>(u =>
                    u.Email == request.Email &&
                    u.CreatedAt != default)),
            Times.Once);

        _stringGeneratorMock.Verify(
            x => x.GenerateRandomString(tokenLength),
            Times.Once);

        _emailTemplateMock.Verify(
            x => x.BuildVerificationEmailContent(
                request.Email, token, verificationLink),
            Times.Once);

        _emailServiceMock.Verify(
            x => x.SendEmailAsync(
                request.Email, subject, body),
            Times.Once);

        _accountVerificationRepositoryMock.Verify(
            x => x.NewAccountVerificationToken(
                It.Is<AccountVerificationEntity>(v =>
                    v.UserId == userId &&
                    v.Token == token &&
                    !v.IsUsed &&
                    v.CreatedAt != default)),
            Times.Once);
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldReturnZero_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new NewUserRequest
        {
            Email = "existing@example.com"
        };

        _userRepositoryMock
            .Setup(x => x.IsAvailableUserByEmail(request.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.Equal(0, result);

        _userRepositoryMock.Verify(
            x => x.RegesterNewUser(It.IsAny<UserEntity>()),
            Times.Never);

        _stringGeneratorMock.Verify(
            x => x.GenerateRandomString(It.IsAny<int>()),
            Times.Never);

        _emailServiceMock.Verify(
            x => x.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _accountVerificationRepositoryMock.Verify(
            x => x.NewAccountVerificationToken(
                It.IsAny<AccountVerificationEntity>()),
            Times.Never);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task RegisterAsync_ShouldReturnZero_WhenUserCreationFails(
        int userId)
    {
        // Arrange
        var request = new NewUserRequest
        {
            Email = "test@example.com"
        };

        _userRepositoryMock
            .Setup(x => x.IsAvailableUserByEmail(request.Email))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.RegesterNewUser(It.IsAny<UserEntity>()))
            .ReturnsAsync(userId);

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.Equal(0, result);

        _stringGeneratorMock.Verify(
            x => x.GenerateRandomString(It.IsAny<int>()),
            Times.Never);

        _emailServiceMock.Verify(
            x => x.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _accountVerificationRepositoryMock.Verify(
            x => x.NewAccountVerificationToken(
                It.IsAny<AccountVerificationEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        NewUserRequest request = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _userService.RegisterAsync(request));

        _userRepositoryMock.Verify(
            x => x.IsAvailableUserByEmail(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldNotSaveToken_WhenEmailSendingFails()
    {
        // Arrange
        var request = new NewUserRequest
        {
            Email = "test@example.com"
        };

        _userRepositoryMock
            .Setup(x => x.IsAvailableUserByEmail(request.Email))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.RegesterNewUser(It.IsAny<UserEntity>()))
            .ReturnsAsync(123);

        _appSettingProviderMock
            .Setup(x => x.GetInt(
                "EmailVerificationAccountSettings:TokenLength"))
            .Returns(32);

        _stringGeneratorMock
            .Setup(x => x.GenerateRandomString(32))
            .Returns("ABC123XYZ456");

        _appSettingProviderMock
            .Setup(x => x.GetString("FrontEnd:BaseUrl"))
            .Returns("https://mywallet.com");

        _emailTemplateMock
            .Setup(x => x.BuildVerificationEmailContent(
                request.Email,
                "ABC123XYZ456",
                "https://mywallet.com/verify-account?token=ABC123XYZ456"))
            .Returns(("Verify account", "Email body"));

        _emailServiceMock
            .Setup(x => x.SendEmailAsync(
                request.Email,
                "Verify account",
                "Email body"))
            .ThrowsAsync(new InvalidOperationException("Email failed"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.RegisterAsync(request));

        _accountVerificationRepositoryMock.Verify(
            x => x.NewAccountVerificationToken(
                It.IsAny<AccountVerificationEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldPropagateException_WhenSavingTokenFails()
    {
        // Arrange
        var request = new NewUserRequest
        {
            Email = "test@example.com"
        };

        _userRepositoryMock
            .Setup(x => x.IsAvailableUserByEmail(request.Email))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.RegesterNewUser(It.IsAny<UserEntity>()))
            .ReturnsAsync(123);

        _appSettingProviderMock
            .Setup(x => x.GetInt(
                "EmailVerificationAccountSettings:TokenLength"))
            .Returns(32);

        _stringGeneratorMock
            .Setup(x => x.GenerateRandomString(32))
            .Returns("ABC123XYZ456");

        _appSettingProviderMock
            .Setup(x => x.GetString("FrontEnd:BaseUrl"))
            .Returns("https://mywallet.com");

        _emailTemplateMock
            .Setup(x => x.BuildVerificationEmailContent(
                request.Email,
                "ABC123XYZ456",
                "https://mywallet.com/verify-account?token=ABC123XYZ456"))
            .Returns(("Verify account", "Email body"));

        _emailServiceMock
            .Setup(x => x.SendEmailAsync(
                request.Email,
                "Verify account",
                "Email body"))
            .Returns(Task.CompletedTask);

        _accountVerificationRepositoryMock
            .Setup(x => x.NewAccountVerificationToken(
                It.IsAny<AccountVerificationEntity>()))
            .ThrowsAsync(new InvalidOperationException("Database failed"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.RegisterAsync(request));
    }
    
}