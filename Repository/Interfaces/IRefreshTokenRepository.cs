using Data.Entities;

namespace Repository.Interfaces;

public interface IRefreshTokenRepository
	: IBaseRepository<RefreshToken>
{
	Task<RefreshToken?> GetByTokenAsync(
		string token,
		CancellationToken cancellationToken = default);
}