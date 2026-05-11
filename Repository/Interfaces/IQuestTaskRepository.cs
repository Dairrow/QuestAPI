using Data.Entities;

namespace Repository.Interfaces;

public interface IQuestTaskRepository
	: IBaseRepository<QuestTask>
{
	Task<IReadOnlyCollection<QuestTask>> GetByQuestIdAsync(
		int questId,
		CancellationToken cancellationToken = default);
}