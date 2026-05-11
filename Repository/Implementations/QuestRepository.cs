using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Interfaces;

namespace Repository.Implementations;

public class QuestRepository
	: BaseRepository<Quest>,
	  IQuestRepository
{
	public QuestRepository(
		AppDbContext context)
		: base(context)
	{
	}


	public async Task<Quest?> GetWithTasksAsync(
		int questId,
		CancellationToken cancellationToken = default)
	{
		return await DbSet
			.AsNoTracking()
			.Include(x => x.Tasks)
			.FirstOrDefaultAsync(
				x => x.Id == questId,
				cancellationToken);
	}
}