using Data.Entities;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;
using Services.Interfaces;

namespace Services.Implementations;

public class RewardService : IRewardService
{
	private readonly IRewardRepository _rewardRepository;

	private readonly ILogger<RewardService> _logger;


	public RewardService(
		IRewardRepository rewardRepository,
		ILogger<RewardService> logger)
	{
		_rewardRepository = rewardRepository;
		_logger = logger;
	}


	public async Task<IReadOnlyCollection<Reward>> GetAllAsync(
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Getting all rewards");

		return await _rewardRepository
			.GetAllAsync(cancellationToken);
	}
}