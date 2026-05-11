using Data.Entities;
using Repository.Context;
using Repository.Interfaces;

namespace Repository.Implementations;

public class RewardRepository
	: BaseRepository<Reward>,
	  IRewardRepository
{
	public RewardRepository(
		AppDbContext context)
		: base(context)
	{
	}
}