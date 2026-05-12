using Data.Entities;

namespace Repository.Interfaces;

public interface IUserRewardRepository : IBaseRepository<UserReward>
{
	Task<IReadOnlyCollection<UserReward>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}