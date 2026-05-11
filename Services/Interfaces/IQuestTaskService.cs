using Data.Entities;

namespace Services.Interfaces;

public interface IQuestTaskService
{
	Task<IReadOnlyCollection<QuestTask>>
		GetByQuestIdAsync(
		int questId,
		CancellationToken cancellationToken = default);
}