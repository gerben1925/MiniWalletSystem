using MiniWalletSystemApi.Entities;
using MiniWalletSystemApi.Interfaces.infrastructure.Security;
using MiniWalletSystemApi.Interfaces.Repositories;
using MiniWalletSystemApi.Payloads.Requests;
using MiniWalletSystemApi.Payloads.Responses;
using MiniWalletSystemApi.Services;
using Moq;
using Xunit;

namespace MiniWalletSystem.UnitTest.Services;

public class UserServiceTests
{
    
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _userService = new UserService(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Username = "gerben",
            Password = "Password123!",
            FullName = "Gerben Clapero",
            Email = "gerben@example.com"
        };

        _userRepositoryMock
            .Setup(x => x.GetByUsernameAsync(request.Username))
            .ReturnsAsync((User?)null);

        _passwordHasherMock
            .Setup(x => x.HashPassword(request.Password))
            .Returns(new PasswordHashResponse
            {
                PasswordHash = "hashed-password",
                PasswordSalt = new byte[] { 1, 2, 3 }
            });

        var createdUser = new UserEntity()
        {
            Id = 1,
            Username = request.Username,
            PasswordHash = "hashed-password",
            PasswordSalt = new byte[] { 1, 2, 3 },
            FullName = request.FullName,
            Email = request.Email,
            RoleID = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UserReference = Guid.NewGuid()
        };

        _userRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        Assert.Equal("gerben", result.Data.Username);
        Assert.Equal("gerben@example.com", result.Data.Email);
        Assert.Equal("hashed-password", result.Data.PasswordHash);
        Assert.Equal(1, result.Data.RoleID);
        Assert.True(result.Data.IsActive);

        _userRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<User>()),
            Times.Once);

        _passwordHasherMock.Verify(
            x => x.Hash(request.Password),
            Times.Once);
    }
    
}