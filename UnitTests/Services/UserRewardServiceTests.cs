using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Repository.Interfaces;
using Services.Exceptions;
using Services.Implementations;
using Xunit;

namespace UnitTests.Services
{
	public class UserRewardServiceTests
	{
		private readonly Mock<IUserRewardRepository> _userRewardRepoMock = new();
		private readonly Mock<IUserQuestRepository> _userQuestRepoMock = new();
		private readonly Mock<IQuestRepository> _questRepoMock = new();
		private readonly UserRewardService _service;

		public UserRewardServiceTests()
		{
			_service = new UserRewardService(
				_userRewardRepoMock.Object,
				_userQuestRepoMock.Object,
				_questRepoMock.Object,
				new Mock<ILogger<UserRewardService>>().Object);
		}

		[Fact]
		public async Task ClaimAsync_ValidClaim_SetsIsClaimedTrue()
		{
			var userReward = new UserReward { Id = 1, UserId = 1, RewardId = 10, IsClaimed = false };
			_userRewardRepoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userReward);

			var completedQuest = new UserQuest { UserId = 1, QuestId = 5, IsCompleted = true };
			_userQuestRepoMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<UserQuest, bool>>>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<UserQuest> { completedQuest });
			var quest = new Quest { Id = 5, RewardId = 10 };
			_questRepoMock.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(quest);

			var result = await _service.ClaimAsync(1);

			Assert.True(result.IsClaimed);
			_userRewardRepoMock.Verify(x => x.Update(userReward), Times.Once);
		}

		[Fact]
		public async Task ClaimAsync_AlreadyClaimed_ThrowsValidationException()
		{
			var userReward = new UserReward { Id = 1, UserId = 1, RewardId = 10, IsClaimed = true };
			_userRewardRepoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userReward);

			await Assert.ThrowsAsync<ValidationException>(() => _service.ClaimAsync(1));
		}

		[Fact]
		public async Task ClaimAsync_NoCompletedQuest_ThrowsValidationException()
		{
			var userReward = new UserReward { Id = 1, UserId = 1, RewardId = 10, IsClaimed = false };
			_userRewardRepoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userReward);
			_userQuestRepoMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<UserQuest, bool>>>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<UserQuest>());

			await Assert.ThrowsAsync<ValidationException>(() => _service.ClaimAsync(1));
		}
	}
}