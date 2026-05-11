using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Interfaces;

namespace Repository.Implementations;

public class RefreshTokenRepository
	: BaseRepository<RefreshToken>,
	  IRefreshTokenRepository
{
	public RefreshTokenRepository(
		AppDbContext context)
		: base(context)
	{
	}


	public async Task<RefreshToken?>
		GetByTokenAsync(
		string token,
		CancellationToken cancellationToken = default)
	{
		return await DbSet
			.FirstOrDefaultAsync(
				x => x.Token == token,
				cancellationToken);
	}
}