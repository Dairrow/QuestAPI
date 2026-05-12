using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Data.Entities;
using Data.Enums;
using Moq;
using Repository.Interfaces;
using Services.Exceptions;
using Services.Implementations;
using Services.Interfaces;
using Services.Models;
using Xunit;

namespace UnitTests.Services
{
	public class AuthServiceTests
	{
		private readonly Mock<IUserRepository> _userRepoMock = new();
		private readonly Mock<IRefreshTokenRepository> _refreshRepoMock = new();
		private readonly Mock<ITokenService> _tokenServiceMock = new();
		private readonly AuthService _authService;

		public AuthServiceTests()
		{
			_authService = new AuthService(
				_userRepoMock.Object,
				_refreshRepoMock.Object,
				_tokenServiceMock.Object);
		}

		[Fact]
		public async Task RegisterAsync_ValidInput_ReturnsAuthResult()
		{
			_userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((User?)null);
			_tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>())).Returns("access");
			_tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh");

			var result = await _authService.RegisterAsync("test", "test@test.com", "Pass1234", UserRole.User);

			Assert.Equal("test", result.Username);
			Assert.Equal("access", result.AccessToken);
			Assert.Equal("refresh", result.RefreshToken);
			_userRepoMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
			_userRepoMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
		}

		[Fact]
		public async Task RegisterAsync_ExistingEmail_ThrowsValidationException()
		{
			_userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new User());

			await Assert.ThrowsAsync<ValidationException>(() =>
				_authService.RegisterAsync("test", "existing@test.com", "Pass1234", UserRole.User));
		}

		[Fact]
		public async Task LoginAsync_ValidCredentials_ReturnsAuthResult_And_RevokesOldTokens()
		{
			var user = new User { Id = 1, Username = "player", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass1234") };
			_userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);
			_tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>())).Returns("access2");
			_tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh2");

			var result = await _authService.LoginAsync("player@test.com", "Pass1234");

			Assert.Equal("access2", result.AccessToken);
			_refreshRepoMock.Verify(x => x.RevokeAllForUserAsync(1, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task LoginAsync_WrongPassword_ThrowsValidationException()
		{
			var user = new User { PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass1234") };
			_userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			await Assert.ThrowsAsync<ValidationException>(() =>
				_authService.LoginAsync("a@a.com", "WrongPass"));
		}

		[Fact]
		public async Task RefreshAsync_ValidToken_ReturnsNewAuthResult_And_RevokesOld()
		{
			var oldToken = new RefreshToken
			{
				Id = 1,
				Token = "oldRefresh",
				UserId = 1,
				IsRevoked = false,
				ExpiresAt = DateTime.UtcNow.AddDays(1)
			};
			var user = new User { Id = 1, Username = "user" };
			_refreshRepoMock.Setup(x => x.GetByTokenAsync("oldRefresh", It.IsAny<CancellationToken>()))
				.ReturnsAsync(oldToken);
			_userRepoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
			_tokenServiceMock.Setup(x => x.GenerateAccessToken(user)).Returns("newAccess");
			_tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("newRefresh");

			var result = await _authService.RefreshAsync("oldRefresh");

			Assert.True(oldToken.IsRevoked);
			Assert.Equal("newAccess", result.AccessToken);
			_refreshRepoMock.Verify(x => x.Update(oldToken), Times.Once);
		}

		[Fact]
		public async Task RefreshAsync_RevokedToken_ThrowsValidationException()
		{
			var token = new RefreshToken { IsRevoked = true, ExpiresAt = DateTime.UtcNow.AddDays(1) };
			_refreshRepoMock.Setup(x => x.GetByTokenAsync("revoked", It.IsAny<CancellationToken>())).ReturnsAsync(token);
			await Assert.ThrowsAsync<ValidationException>(() => _authService.RefreshAsync("revoked"));
		}
	}
}