using Data.Entities;

namespace Services.Interfaces;

public interface IQuestTaskService
{
	Task<IReadOnlyCollection<QuestTask>>
		GetByQuestIdAsync(
			int questId,
			CancellationToken cancellationToken = default);

	Task<QuestTask>
		GetByIdAsync(
			int id,
			CancellationToken cancellationToken = default);

	Task<QuestTask>
		CreateAsync(
			QuestTask entity,
			CancellationToken cancellationToken = default);

	Task<QuestTask>
		UpdateAsync(
			int id,
			QuestTask entity,
			CancellationToken cancellationToken = default);

	Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default);
}