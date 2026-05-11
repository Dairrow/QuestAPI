using Data.Entities;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;
using Services.Interfaces;

namespace Services.Implementations;

public class QuestTaskService
	: IQuestTaskService
{
	private readonly IQuestTaskRepository _repository;

	private readonly ILogger<QuestTaskService>
		_logger;


	public QuestTaskService(
		IQuestTaskRepository repository,
		ILogger<QuestTaskService> logger)
	{
		_repository = repository;
		_logger = logger;
	}


	public async Task<IReadOnlyCollection<QuestTask>>
		GetByQuestIdAsync(
		int questId,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Getting quest tasks");

		return await _repository
			.GetByQuestIdAsync(
				questId,
				cancellationToken);
	}
}