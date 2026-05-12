using Data.Entities;

namespace Services.Interfaces;

public interface IUserRewardService
{
	Task<IReadOnlyCollection<UserReward>>
		GetByUserIdAsync(
			int userId,
			CancellationToken cancellationToken = default);

	Task<UserReward>
		GetByIdAsync(
			int id,
			CancellationToken cancellationToken = default);

	Task<UserReward>
		CreateAsync(
			UserReward entity,
			CancellationToken cancellationToken = default);

	Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default);
}