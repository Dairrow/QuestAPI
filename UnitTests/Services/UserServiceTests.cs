using System.Threading;
using System.Threading.Tasks;
using Data.Entities;
using Data.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Repository.Interfaces;
using Services.Exceptions;
using Services.Implementations;
using Xunit;

namespace UnitTests.Services
{
	public class UserServiceTests
	{
		private readonly Mock<IUserRepository> _repoMock = new();
		private readonly UserService _service;

		public UserServiceTests()
		{
			_service = new UserService(_repoMock.Object, new Mock<ILogger<UserService>>().Object);
		}

		[Fact]
		public async Task GetByIdAsync_UserExists_ReturnsUser()
		{
			var user = new User { Id = 1, Username = "admin" };
			_repoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);

			var result = await _service.GetByIdAsync(1);

			Assert.Equal("admin", result.Username);
		}

		[Fact]
		public async Task GetByIdAsync_UserNotFound_ThrowsNotFoundException()
		{
			_repoMock.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
			await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(999));
		}

		[Fact]
		public async Task UpdateAsync_UpdatesFields_And_ReturnsUpdatedUser()
		{
			var existing = new User { Id = 1, Username = "old", Email = "old@mail.com", PasswordHash = "hash", Role = UserRole.User };
			_repoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

			var updated = new User { Username = "new", Email = "new@mail.com", PasswordHash = "newHash", Role = UserRole.Admin };

			var result = await _service.UpdateAsync(1, updated);

			Assert.Equal("new", result.Username);
			Assert.Equal("new@mail.com", result.Email);
			_repoMock.Verify(x => x.Update(existing), Times.Once);
		}

		[Fact]
		public async Task ChangePasswordAsync_ValidOldPassword_ChangesPassword()
		{
			var user = new User { Id = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldPass") };
			_repoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);

			await _service.ChangePasswordAsync(1, "oldPass", "newPass");
			_repoMock.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
		}

		[Fact]
		public async Task ChangePasswordAsync_WrongOldPassword_ThrowsValidationException()
		{
			var user = new User { Id = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldPass") };
			_repoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);

			await Assert.ThrowsAsync<ValidationException>(() =>
				_service.ChangePasswordAsync(1, "wrongOld", "newPass"));
		}
	}
}