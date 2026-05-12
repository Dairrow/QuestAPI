using Data.Entities;
using Microsoft.Extensions.Logging;
using Repository.Implementations;
using Repository.Interfaces;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations;

public class UserRewardService : IUserRewardService
{
	private readonly IUserRewardRepository _userRewardRepository;
	private readonly IUserQuestRepository _userQuestRepository;
	private readonly IQuestRepository _questRepository;
	private readonly ILogger<UserRewardService> _logger;

	public UserRewardService(
		IUserRewardRepository userRewardRepository,
		IUserQuestRepository userQuestRepository,
		IQuestRepository questRepository,
		ILogger<UserRewardService> logger)
	{
		_userRewardRepository = userRewardRepository;
		_userQuestRepository = userQuestRepository;
		_questRepository = questRepository;
		_logger = logger;
	}

	public async Task<IReadOnlyCollection<UserReward>> GetByUserIdAsync(
		int userId,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Getting rewards for user {UserId}",
			userId);

		return await _userRewardRepository
			.GetByUserIdAsync(userId, cancellationToken);
	}

	public async Task<UserReward> GetByIdAsync(
		int id,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Getting user reward {UserRewardId}",
			id);

		var userReward = await _userRewardRepository
			.GetByIdAsync(id, cancellationToken);

		if (userReward is null)
		{
			throw new NotFoundException(
				$"UserReward {id} not found");
		}

		return userReward;
	}

	public async Task<UserReward> CreateAsync(
		UserReward entity,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Giving reward {RewardId} to user {UserId}",
			entity.RewardId,
			entity.UserId);

		var existingRewards = await _userRewardRepository
			.GetByUserIdAsync(entity.UserId, cancellationToken);

		if (existingRewards.Any(r => r.RewardId == entity.RewardId))
		{
			throw new InvalidOperationException(
				$"Reward {entity.RewardId} already given to user {entity.UserId}");
		}

		await _userRewardRepository.AddAsync(
			entity,
			cancellationToken);

		await _userRewardRepository.SaveChangesAsync(
			cancellationToken);

		return entity;
	}

	public async Task<UserReward> ClaimAsync(int userRewardId, CancellationToken cancellationToken = default)
	{
		var userReward = await _userRewardRepository.GetByIdAsync(userRewardId, cancellationToken)
			?? throw new NotFoundException($"UserReward {userRewardId} not found");

		if (userReward.IsClaimed)
			throw new ValidationException("Reward already claimed");

		var completedQuest = await _userQuestRepository.FindAsync(
			uq => uq.UserId == userReward.UserId && uq.IsCompleted,
			cancellationToken);

		var rewardId = userReward.RewardId;

		bool hasMatchingQuest = false;
		foreach (var uq in completedQuest)
		{
			var quest = await _questRepository.GetByIdAsync(uq.QuestId, cancellationToken);
			if (quest?.RewardId == rewardId)
			{
				hasMatchingQuest = true;
				break;
			}
		}

		if (!hasMatchingQuest)
			throw new ValidationException("No completed quest gives this reward");

		userReward.IsClaimed = true;
		_userRewardRepository.Update(userReward);
		await _userRewardRepository.SaveChangesAsync(cancellationToken);

		_logger.LogInformation("User {UserId} claimed reward {RewardId} (UserReward {Id})",
			userReward.UserId, userReward.RewardId, userReward.Id);

		return userReward;
	}

	public async Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Removing user reward {UserRewardId}",
			id);

		var entity = await GetByIdAsync(id, cancellationToken);

		_userRewardRepository.Delete(entity);
		await _userRewardRepository.SaveChangesAsync(cancellationToken);
	}
}