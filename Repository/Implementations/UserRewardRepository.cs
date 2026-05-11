using Data.Entities;
using Repository.Context;
using Repository.Interfaces;

namespace Repository.Implementations;

public class UserRewardRepository
	: BaseRepository<UserReward>,
	  IUserRewardRepository
{
	public UserRewardRepository(
		AppDbContext context)
		: base(context)
	{
	}
}