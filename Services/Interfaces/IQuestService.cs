using Data.Entities;

namespace Services.Interfaces;

public interface IQuestService
{
	Task<IReadOnlyCollection<Quest>>
		GetAllAsync(
			CancellationToken cancellationToken = default);

	Task<Quest>
		GetByIdAsync(
			int id,
			CancellationToken cancellationToken = default);

	Task<Quest>
		CreateAsync(
			Quest entity,
			CancellationToken cancellationToken = default);

	Task<Quest>
		UpdateAsync(
			int id,
			Quest entity,
			CancellationToken cancellationToken = default);

	Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default);
}