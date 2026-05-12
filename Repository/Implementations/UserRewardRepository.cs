using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Interfaces;

namespace Repository.Implementations;

public class UserRewardRepository : BaseRepository<UserReward>, IUserRewardRepository
{
	public UserRewardRepository(AppDbContext context) : base(context) { }

	public async Task<IReadOnlyCollection<UserReward>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
	{
		return await DbSet
			.AsNoTracking()
			.Where(ur => ur.UserId == userId)
			.ToListAsync(cancellationToken);
	}
}