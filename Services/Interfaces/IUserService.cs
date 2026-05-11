using Data.Entities;

namespace Services.Interfaces;

public interface IUserService
{
	Task<IReadOnlyCollection<User>> GetAllAsync(
		CancellationToken cancellationToken = default);

	Task<User> GetByIdAsync(
		int id,
		CancellationToken cancellationToken = default);
}