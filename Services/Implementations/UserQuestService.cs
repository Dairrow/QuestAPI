using Data.Entities;
using Microsoft.Extensions.Logging;
using Repository.Implementations;
using Repository.Interfaces;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations;

public class UserQuestService : IUserQuestService
{
	private readonly IUserQuestRepository _userQuestRepository;
	private readonly IQuestRepository _questRepository;
	private readonly IUserRewardRepository _userRewardRepository;
	private readonly ILogger<UserQuestService> _logger;

	public UserQuestService(
		IUserQuestRepository userQuestRepository,
		IQuestRepository questRepository,
		IUserRewardRepository userRewardRepository,
		ILogger<UserQuestService> logger)
	{
		_userQuestRepository = userQuestRepository;
		_questRepository = questRepository;
		_userRewardRepository = userRewardRepository;
		_logger = logger;
	}

	public async Task<IReadOnlyCollection<UserQuest>> GetByUserIdAsync(
		int userId,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Getting quests for user {UserId}",
			userId);

		return await _userQuestRepository
			.GetByUserIdAsync(userId, cancellationToken);
	}

	public async Task<UserQuest> GetByIdAsync(
		int id,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Getting user quest {UserQuestId}",
			id);

		var userQuest = await _userQuestRepository
			.GetByIdAsync(id, cancellationToken);

		if (userQuest is null)
		{
			throw new NotFoundException(
				$"UserQuest {id} not found");
		}

		return userQuest;
	}

	public async Task<UserQuest> CreateAsync(
		UserQuest entity,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Assigning quest {QuestId} to user {UserId}",
			entity.QuestId,
			entity.UserId);

		var existingQuests = await _userQuestRepository
			.GetByUserIdAsync(entity.UserId, cancellationToken);

		if (existingQuests.Any(q => q.QuestId == entity.QuestId))
		{
			throw new InvalidOperationException(
				$"Quest {entity.QuestId} already assigned to user {entity.UserId}");
		}

		entity.IsCompleted = false;

		await _userQuestRepository.AddAsync(
			entity,
			cancellationToken);

		await _userQuestRepository.SaveChangesAsync(
			cancellationToken);

		return entity;
	}

	public async Task<UserQuest> UpdateAsync(
		int id,
		UserQuest entity,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Updating user quest {UserQuestId}",
			id);

		var existing = await GetByIdAsync(id, cancellationToken);

		existing.IsCompleted = entity.IsCompleted;

		existing.UpdatedAt = DateTime.UtcNow;

		_userQuestRepository.Update(existing);
		await _userQuestRepository.SaveChangesAsync(cancellationToken);

		return existing;
	}

	public async Task<UserQuest> UpdateProgressAsync(int userQuestId, int taskOrder, CancellationToken cancellationToken = default)
	{
		var userQuest = await _userQuestRepository.GetByIdAsync(userQuestId, cancellationToken)
			?? throw new NotFoundException($"UserQuest {userQuestId} not found");

		var quest = await _questRepository.GetWithTasksAsync(userQuest.QuestId, cancellationToken)
			?? throw new NotFoundException($"Quest {userQuest.QuestId} not found");

		var tasks = quest.Tasks.OrderBy(t => t.Order).ToList();
		if (tasks.Count == 0)
			throw new ForbiddenException("Quest has no tasks");

		if (taskOrder == 0)
		{
			if (!userQuest.IsCompleted)
				throw new ForbiddenException("Quest is not completed, cannot reset to 0");

			userQuest.IsCompleted = false;
			userQuest.LastCompletedTaskOrder = null;

			if (quest.RewardId != null)
			{
				var userRewards = await _userRewardRepository.FindAsync(
					ur => ur.UserId == userQuest.UserId && ur.RewardId == quest.RewardId.Value,
					cancellationToken);
				var userReward = userRewards.FirstOrDefault();
				if (userReward != null)
				{
					userReward.IsClaimed = false;
					_userRewardRepository.Update(userReward);
				}
			}

			_userQuestRepository.Update(userQuest);
			await _userQuestRepository.SaveChangesAsync(cancellationToken);
			return userQuest;
		}

		int nextOrder = (userQuest.LastCompletedTaskOrder ?? 0) + 1;
		if (taskOrder != nextOrder)
			throw new ForbiddenException($"Invalid task order. Expected {nextOrder}");

		var task = tasks.FirstOrDefault(t => t.Order == taskOrder)
			?? throw new NotFoundException($"Task with order {taskOrder} not found in quest");

		userQuest.LastCompletedTaskOrder = taskOrder;

		if (taskOrder == tasks.Last().Order)
		{
			userQuest.IsCompleted = true;

			if (quest.RewardId != null)
			{
				var existingRewards = await _userRewardRepository.FindAsync(
					ur => ur.UserId == userQuest.UserId && ur.RewardId == quest.RewardId.Value,
					cancellationToken);

				var userReward = existingRewards.FirstOrDefault();
				if (userReward == null)
				{
					userReward = new UserReward
					{
						UserId = userQuest.UserId,
						RewardId = quest.RewardId.Value,
						ReceivedAt = DateTime.UtcNow,
						IsClaimed = false
					};
					await _userRewardRepository.AddAsync(userReward, cancellationToken);
				}
				else
				{
					userReward.IsClaimed = false;
					_userRewardRepository.Update(userReward);
				}
			}
		}
		else
		{
			userQuest.IsCompleted = false;
		}

		userQuest.UpdatedAt = DateTime.UtcNow;

		_userQuestRepository.Update(userQuest);
		await _userQuestRepository.SaveChangesAsync(cancellationToken);
		return userQuest;
	}

	public async Task<bool> HasUserQuestAsync(
		int userId, 
		int questId, 
		CancellationToken cancellationToken = default)
	{
		return await _userQuestRepository.ExistsAsync(
			uq => uq.UserId == userId && 
			uq.QuestId == questId, cancellationToken);
	}

	public async Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Removing user quest {UserQuestId}",
			id);

		var entity = await GetByIdAsync(id, cancellationToken);

		_userQuestRepository.Delete(entity);
		await _userQuestRepository.SaveChangesAsync(cancellationToken);
	}
}