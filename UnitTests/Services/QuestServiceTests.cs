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
	public class QuestServiceTests
	{
		private readonly Mock<IQuestRepository> _repoMock = new();
		private readonly QuestService _service;

		public QuestServiceTests()
		{
			_service = new QuestService(_repoMock.Object, new Mock<ILogger<QuestService>>().Object);
		}

		[Fact]
		public async Task GetByIdAsync_QuestWithTasks_ReturnsQuest()
		{
			var quest = new Quest { Id = 1, Title = "Epic Quest" };
			_repoMock.Setup(x => x.GetWithTasksAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(quest);

			var result = await _service.GetByIdAsync(1);
			Assert.Equal("Epic Quest", result.Title);
		}

		[Fact]
		public async Task GetByIdAsync_NotFound_ThrowsNotFoundException()
		{
			_repoMock.Setup(x => x.GetWithTasksAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Quest?)null);
			await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
		}

		[Fact]
		public async Task UpdateAsync_UpdatesFields()
		{
			var existing = new Quest { Id = 1, Title = "Old", Description = "Old", Difficulty = QuestDifficulty.Easy };
			_repoMock.Setup(x => x.GetWithTasksAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
			_repoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

			var updateData = new Quest { Title = "New", Description = "New Desc", Difficulty = QuestDifficulty.Hard, RewardId = 2 };

			var result = await _service.UpdateAsync(1, updateData);
			Assert.Equal("New", result.Title);
			Assert.Equal(2, result.RewardId);
		}
	}
}