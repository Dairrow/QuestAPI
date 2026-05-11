using Data.Entities;
using Repository.Context;
using Repository.Interfaces;

namespace Repository.Implementations;

public class UserQuestRepository
	: BaseRepository<UserQuest>,
	  IUserQuestRepository
{
	public UserQuestRepository(
		AppDbContext context)
		: base(context)
	{
	}
}