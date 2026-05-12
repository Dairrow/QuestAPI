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

	public async Task<Quest>
	CreateAsync(
	Quest entity,
	CancellationToken cancellationToken = default)
	{
		await _questRepository.AddAsync(
			entity,
			cancellationToken);

		await _questRepository.SaveChangesAsync(
			cancellationToken);

		return entity;
	}


	public async Task<Quest>
		UpdateAsync(
		int id,
		Quest entity,
		CancellationToken cancellationToken = default)
	{
		var existing =
			await GetByIdAsync(
				id,
				cancellationToken);


		existing.Title =
			entity.Title;

		existing.Description =
			entity.Description;

		existing.Difficulty = 
			entity.Difficulty;

		existing.RewardId = 
			entity.RewardId;

		existing.UpdatedAt =
			DateTime.UtcNow;

		_questRepository.Update(
			existing);

		await _questRepository.SaveChangesAsync(
			cancellationToken);

		return existing;
	}


	public async Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default)
	{
		var entity =
			await GetByIdAsync(
				id,
				cancellationToken);


		_questRepository.Delete(
			entity);

		await _questRepository.SaveChangesAsync(
			cancellationToken);
	}
}