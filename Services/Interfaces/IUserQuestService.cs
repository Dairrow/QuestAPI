using Data.Entities;

namespace Services.Interfaces;

public interface IUserQuestService
{
	Task<IReadOnlyCollection<UserQuest>>
		GetByUserIdAsync(
			int userId,
			CancellationToken cancellationToken = default);

	Task<UserQuest>
		GetByIdAsync(
			int id,
			CancellationToken cancellationToken = default);

	Task<UserQuest>
		CreateAsync(
			UserQuest entity,
			CancellationToken cancellationToken = default);

	Task<UserQuest>
		UpdateAsync(
			int id,
			UserQuest entity,
			CancellationToken cancellationToken = default);

	Task<UserQuest> 
		UpdateProgressAsync(int userQuestId, 
		int taskOrder, 
		CancellationToken cancellationToken = default);

	Task<bool> 
		HasUserQuestAsync(int userId, 
		int questId, 
		CancellationToken cancellationToken = default);

	Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default);
}