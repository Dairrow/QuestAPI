using Data.Entities;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations;

public class QuestService : IQuestService
{
	private readonly IQuestRepository _questRepository;

	private readonly ILogger<QuestService> _logger;


	public QuestService(
		IQuestRepository questRepository,
		ILogger<QuestService> logger)
	{
		_questRepository = questRepository;
		_logger = logger;
	}


	public async Task<IReadOnlyCollection<Quest>> GetAllAsync(
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Getting all quests");

		return await _questRepository
			.GetAllAsync(cancellationToken);
	}


	public async Task<Quest> GetByIdAsync(
		int id,
		CancellationToken cancellationToken = default)
	{
		var quest = await _questRepository
			.GetWithTasksAsync(
				id,
				cancellationToken);


		if (quest is null)
		{
			throw new NotFoundException(
				$"Quest {id} not found");
		}


		return quest;
	}
}