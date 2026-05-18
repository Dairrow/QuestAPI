using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Interfaces;

namespace Repository.Implementations;

public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
{
	public InventoryRepository(AppDbContext context) : base(context) { }

	public async Task<IReadOnlyCollection<Inventory>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
	{
		return await DbSet
			.AsNoTracking()
			.Where(ur => ur.UserId == userId)
			.ToListAsync(cancellationToken);
	}
}