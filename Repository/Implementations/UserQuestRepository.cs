using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Interfaces;

namespace Repository.Implementations;

public class UserQuestRepository : BaseRepository<UserQuest>, IUserQuestRepository
{
	public UserQuestRepository(AppDbContext context) : base(context) { }

	public async Task<IReadOnlyCollection<UserQuest>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
	{
		return await DbSet
			.AsNoTracking()
			.Where(uq => uq.UserId == userId)
			.ToListAsync(cancellationToken);
	}
}