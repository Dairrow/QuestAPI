using Data.Entities;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations;

public class QuestTaskService : IQuestTaskService
{
	private readonly IQuestTaskRepository _questTaskRepository;
	private readonly ILogger<QuestTaskService> _logger;

	public QuestTaskService(
		IQuestTaskRepository questTaskRepository,
		ILogger<QuestTaskService> logger)
	{
		_questTaskRepository = questTaskRepository;
		_logger = logger;
	}

	public async Task<IReadOnlyCollection<QuestTask>> GetByQuestIdAsync(
		int questId,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Getting tasks for quest {QuestId}",
			questId);

		return await _questTaskRepository
			.GetByQuestIdAsync(questId, cancellationToken);
	}

	public async Task<QuestTask> GetByIdAsync(
		int id,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Getting task {TaskId}",
			id);

		var task = await _questTaskRepository
			.GetByIdAsync(id, cancellationToken);

		if (task is null)
		{
			throw new NotFoundException(
				$"QuestTask {id} not found");
		}

		return task;
	}

	public async Task<QuestTask> CreateAsync(
		QuestTask entity,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Creating task for quest {QuestId}",
			entity.QuestId);

		await _questTaskRepository.AddAsync(
			entity,
			cancellationToken);

		await _questTaskRepository.SaveChangesAsync(
			cancellationToken);

		return entity;
	}

	public async Task<QuestTask> UpdateAsync(
		int id,
		QuestTask entity,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Updating task {TaskId}",
			id);

		var existing = await GetByIdAsync(id, cancellationToken);

		existing.Title = entity.Title;
		existing.Description = entity.Description;
		existing.Order = entity.Order;
		existing.UpdatedAt = DateTime.UtcNow;

		_questTaskRepository.Update(existing);
		await _questTaskRepository.SaveChangesAsync(cancellationToken);

		return existing;
	}

	public async Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Deleting task {TaskId}",
			id);

		var entity = await GetByIdAsync(id, cancellationToken);

		_questTaskRepository.Delete(entity);
		await _questTaskRepository.SaveChangesAsync(cancellationToken);
	}
}