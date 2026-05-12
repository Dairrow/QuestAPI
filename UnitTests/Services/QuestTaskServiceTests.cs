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
	public class QuestTaskServiceTests
	{
		private readonly Mock<IQuestTaskRepository> _repoMock = new();
		private readonly QuestTaskService _service;

		public QuestTaskServiceTests()
		{
			_service = new QuestTaskService(_repoMock.Object, new Mock<ILogger<QuestTaskService>>().Object);
		}

		[Fact]
		public async Task GetByIdAsync_ReturnsTask()
		{
			_repoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new QuestTask { Id = 1, Title = "Task1" });
			var result = await _service.GetByIdAsync(1);
			Assert.Equal("Task1", result.Title);
		}

		[Fact]
		public async Task UpdateAsync_UpdatesOrder()
		{
			var existing = new QuestTask { Id = 1, Title = "Old", Order = 2 };
			_repoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
			var update = new QuestTask { Title = "New", Description = "Desc", Order = 5 };
			var result = await _service.UpdateAsync(1, update);
			Assert.Equal(5, result.Order);
		}
	}
}