using Data.Entities;

namespace Services.Interfaces;

public interface IQuestService
{
	Task<IReadOnlyCollection<Quest>> GetAllAsync(
		CancellationToken cancellationToken = default);

	Task<Quest> GetByIdAsync(
		int id,
		CancellationToken cancellationToken = default);
}