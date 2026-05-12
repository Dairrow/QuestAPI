using Data.Entities;

namespace Services.Interfaces;

public interface IRewardService
{
	Task<IReadOnlyCollection<Reward>>
		GetAllAsync(
			CancellationToken cancellationToken = default);

	Task<Reward>
		GetByIdAsync(
			int id,
			CancellationToken cancellationToken = default);

	Task<Reward>
		CreateAsync(
			Reward entity,
			CancellationToken cancellationToken = default);

	Task<Reward>
		UpdateAsync(
			int id,
			Reward entity,
			CancellationToken cancellationToken = default);

	Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default);
}