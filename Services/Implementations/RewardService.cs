using Data.Entities;
using Microsoft.Extensions.Logging;
using Repository.Implementations;
using Repository.Interfaces;
using Services.Exceptions;
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

	public async Task<Reward> GetByIdAsync(
		int id,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
		"Getting reward {RewardId}",
			id);


		var Reward = await _rewardRepository
			.GetByIdAsync(
				id,
				cancellationToken);


		if (Reward is null)
		{
			throw new NotFoundException(
				$"Reward {id} not found");
		}


		return Reward;
	}

	public async Task<Reward>
	CreateAsync(
	Reward entity,
	CancellationToken cancellationToken = default)
	{
		await _rewardRepository.AddAsync(
			entity,
			cancellationToken);

		await _rewardRepository.SaveChangesAsync(
			cancellationToken);

		return entity;
	}


	public async Task<Reward>
		UpdateAsync(
		int id,
		Reward entity,
		CancellationToken cancellationToken = default)
	{
		var existing =
			await GetByIdAsync(
				id,
				cancellationToken);


		existing.Name =
			entity.Name;

		existing.Type = 
			entity.Type;

		existing.Value = 
			entity.Value;

		existing.ImagePath = 
			entity.ImagePath;

		existing.UpdatedAt =
			DateTime.UtcNow;


		_rewardRepository.Update(
			existing);

		await _rewardRepository.SaveChangesAsync(
			cancellationToken);

		return existing;
	}


	public async Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default)
	{
		var entity =
			await GetByIdAsync(
				id,
				cancellationToken);


		_rewardRepository.Delete(
			entity);

		await _rewardRepository.SaveChangesAsync(
			cancellationToken);
	}
}