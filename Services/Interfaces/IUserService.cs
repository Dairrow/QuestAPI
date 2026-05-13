using Data.Entities;

namespace Services.Interfaces;

public interface IUserService
{
	Task<IReadOnlyCollection<User>>
		GetAllAsync(
			CancellationToken cancellationToken = default);

	Task<User>
		GetByIdAsync(
			int id,
			CancellationToken cancellationToken = default);

	Task
		ChangePasswordAsync(
		int userId,
		string oldPassword,
		string newPassword,
		CancellationToken cancellationToken = default);

	Task<User>
		CreateAsync(
			User entity,
			CancellationToken cancellationToken = default);

	Task<User>
		UpdateAsync(
			int id,
			User entity,
			CancellationToken cancellationToken = default);

	Task<User> 
		AdminUpdateAsync(
			int id,
			User entity,
			CancellationToken cancellationToken = default);

	Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default);
}