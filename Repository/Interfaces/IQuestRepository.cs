using Data.Entities;

namespace Repository.Interfaces;

public interface IQuestRepository
	: IBaseRepository<Quest>
{
	Task<Quest?> GetWithTasksAsync(
		int questId,
		CancellationToken cancellationToken = default);
}