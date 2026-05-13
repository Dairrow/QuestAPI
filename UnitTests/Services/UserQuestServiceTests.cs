using System.Collections.Generic;
using System.Linq;
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
	public class UserQuestServiceTests
	{
		private readonly Mock<IUserQuestRepository> _userQuestRepoMock = new();
		private readonly Mock<IQuestRepository> _questRepoMock = new();
		private readonly Mock<IUserRewardRepository> _userRewardRepoMock = new();
		private readonly UserQuestService _service;

		public UserQuestServiceTests()
		{
			_service = new UserQuestService(
				_userQuestRepoMock.Object,
				_questRepoMock.Object,
				_userRewardRepoMock.Object,
				new Mock<ILogger<UserQuestService>>().Object);
		}

		[Fact]
		public async Task UpdateProgressAsync_ValidNextTask_UpdatesProgress()
		{
			var quest = new Quest
			{
				Id = 1,
				Tasks = new List<QuestTask>
				{
					new QuestTask { Order = 1 },
					new QuestTask { Order = 2 },
					new QuestTask { Order = 3 }
				}
			};
			var userQuest = new UserQuest { Id = 1, UserId = 1, QuestId = 1, LastCompletedTaskOrder = null, IsCompleted = false };
			_userQuestRepoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userQuest);
			_questRepoMock.Setup(x => x.GetWithTasksAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(quest);

			var result = await _service.UpdateProgressAsync(1, 1);

			Assert.Equal(1, result.LastCompletedTaskOrder);
			Assert.False(result.IsCompleted);
		}

		[Fact]
		public async Task UpdateProgressAsync_LastTask_CompletesQuest_And_IssuesReward()
		{
			var quest = new Quest
			{
				Id = 1,
				RewardId = 5,
				Tasks = new List<QuestTask> { new QuestTask { Order = 1 } }
			};
			var userQuest = new UserQuest { Id = 1, UserId = 1, QuestId = 1 };
			_userQuestRepoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userQuest);
			_questRepoMock.Setup(x => x.GetWithTasksAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(quest);
			_userRewardRepoMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<UserReward, bool>>>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<UserReward>());

			var result = await _service.UpdateProgressAsync(1, 1);

			Assert.True(result.IsCompleted);
			_userRewardRepoMock.Verify(x => x.AddAsync(It.Is<UserReward>(r => r.UserId == 1 && r.RewardId == 5), It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task UpdateProgressAsync_ResetCompletedQuest_ResetsAndRemovesRewardClaim()
		{
			var quest = new Quest { Id = 1, RewardId = 3, Tasks = new List<QuestTask> { new QuestTask { Order = 1 } } };
			var userQuest = new UserQuest { Id = 1, UserId = 1, QuestId = 1, IsCompleted = true, LastCompletedTaskOrder = 1 };
			var existingReward = new UserReward { Id = 10, UserId = 1, RewardId = 3, IsClaimed = true };
			_userQuestRepoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userQuest);
			_questRepoMock.Setup(x => x.GetWithTasksAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(quest);
			_userRewardRepoMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<UserReward, bool>>>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<UserReward> { existingReward });

			await _service.UpdateProgressAsync(1, 0);

			Assert.False(userQuest.IsCompleted);
			Assert.Null(userQuest.LastCompletedTaskOrder);
			Assert.False(existingReward.IsClaimed);
			_userRewardRepoMock.Verify(x => x.Update(existingReward), Times.Once);
		}

		[Fact]
		public async Task UpdateProgressAsync_InvalidOrder_ThrowsValidationException()
		{
			var quest = new Quest { Tasks = new List<QuestTask> { new QuestTask { Order = 1 }, new QuestTask { Order = 2 } } };
			var userQuest = new UserQuest { Id = 1, QuestId = 1, LastCompletedTaskOrder = null };
			_userQuestRepoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userQuest);
			_questRepoMock.Setup(x => x.GetWithTasksAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(quest);

			await Assert.ThrowsAsync<ForbiddenException>(() => _service.UpdateProgressAsync(1, 2));
		}

		[Fact]
		public async Task UpdateProgressAsync_ResetWhenNotCompleted_ThrowsValidationException()
		{
			var quest = new Quest { Tasks = new List<QuestTask> { new QuestTask { Order = 1 } } };
			var userQuest = new UserQuest { Id = 1, QuestId = 1, IsCompleted = false };
			_userQuestRepoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(userQuest);
			_questRepoMock.Setup(x => x.GetWithTasksAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(quest);

			await Assert.ThrowsAsync<ForbiddenException>(() => _service.UpdateProgressAsync(1, 0));
		}
	}
}