using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Interfaces;

namespace Repository.Implementations;

public class UserRepository
	: BaseRepository<User>,
	  IUserRepository
{
	public UserRepository(
		AppDbContext context)
		: base(context)
	{
	}


	public async Task<User?> GetByEmailAsync(
		string email,
		CancellationToken cancellationToken = default)
	{
		return await DbSet
			.FirstOrDefaultAsync(
				x => x.Email == email,
				cancellationToken);
	}


	public async Task<User?> GetByUsernameAsync(
		string username,
		CancellationToken cancellationToken = default)
	{
		return await DbSet
			.FirstOrDefaultAsync(
				x => x.Username == username,
				cancellationToken);
	}
}