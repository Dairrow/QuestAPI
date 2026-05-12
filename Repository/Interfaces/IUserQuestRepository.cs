using Data.Entities;

namespace Repository.Interfaces;

public interface IUserQuestRepository : IBaseRepository<UserQuest>
{
	Task<IReadOnlyCollection<UserQuest>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}