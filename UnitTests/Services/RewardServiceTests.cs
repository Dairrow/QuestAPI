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
	public class RewardServiceTests
	{
		private readonly Mock<IRewardRepository> _repoMock = new();
		private readonly RewardService _service;

		public RewardServiceTests()
		{
			_service = new RewardService(_repoMock.Object, new Mock<ILogger<RewardService>>().Object);
		}

		[Fact]
		public async Task GetByIdAsync_ReturnsReward_IfExists()
		{
			_repoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new Reward { Id = 1, Name = "Gold" });
			var result = await _service.GetByIdAsync(1);
			Assert.Equal("Gold", result.Name);
		}

		[Fact]
		public async Task GetByIdAsync_NotFound_ThrowsNotFoundException()
		{
			_repoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Reward?)null);
			await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(1));
		}

		[Fact]
		public async Task UpdateAsync_UpdatesProperties()
		{
			var existing = new Reward { Id = 1, Name = "Old", Type = RewardType.Currency, Value = 10 };
			_repoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
			var update = new Reward { Name = "New", Type = RewardType.Item, Value = 20, ImagePath = "/img/test.jpg" };
			var result = await _service.UpdateAsync(1, update);

			Assert.Equal("New", result.Name);
			Assert.Equal("/img/test.jpg", result.ImagePath);
		}
	}
}