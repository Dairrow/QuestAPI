using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Interfaces;

namespace Repository.Implementations;

public class QuestTaskRepository
	: BaseRepository<QuestTask>,
	  IQuestTaskRepository
{
	public QuestTaskRepository(
		AppDbContext context)
		: base(context)
	{
	}


	public async Task<IReadOnlyCollection<QuestTask>>
		GetByQuestIdAsync(
		int questId,
		CancellationToken cancellationToken = default)
	{
		return await DbSet
			.AsNoTracking()
			.Where(x => x.QuestId == questId)
			.ToListAsync(cancellationToken);
	}
}