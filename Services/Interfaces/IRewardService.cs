using Data.Entities;

namespace Services.Interfaces;

public interface IRewardService
{
	Task<IReadOnlyCollection<Reward>> GetAllAsync(
		CancellationToken cancellationToken = default);
}