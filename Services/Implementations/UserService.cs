using Data.Entities;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;
using Services.Exceptions;
using Services.Interfaces;
using BCrypt.Net;

namespace Services.Implementations;

public class UserService : IUserService
{
	private readonly IUserRepository _userRepository;

	private readonly ILogger<UserService> _logger;


	public UserService(
		IUserRepository userRepository,
		ILogger<UserService> logger)
	{
		_userRepository = userRepository;
		_logger = logger;
	}


	public async Task<IReadOnlyCollection<User>> GetAllAsync(
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Getting all users");

		return await _userRepository
			.GetAllAsync(cancellationToken);
	}


	public async Task<User> GetByIdAsync(
		int id,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation(
			"Getting user {UserId}",
			id);


		var user = await _userRepository
			.GetByIdAsync(
				id,
				cancellationToken);


		if (user is null)
		{
			throw new NotFoundException(
				$"User {id} not found");
		}


		return user;
	}

	public async Task<User>
	CreateAsync(
	User entity,
	CancellationToken cancellationToken = default)
	{
		await _userRepository.AddAsync(
			entity,
			cancellationToken);

		await _userRepository.SaveChangesAsync(
			cancellationToken);

		return entity;
	}


	public async Task<User>
		UpdateAsync(
		int id,
		User entity,
		CancellationToken cancellationToken = default)
	{
		var existing =
			await GetByIdAsync(
				id,
				cancellationToken);


		existing.Username =
			entity.Username;

		existing.Email =
			entity.Email;

		existing.UpdatedAt = 
			DateTime.UtcNow;

		_userRepository.Update(
			existing);

		await _userRepository.SaveChangesAsync(
			cancellationToken);

		return existing;
	}


	public async Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default)
	{
		var entity =
			await GetByIdAsync(
				id,
				cancellationToken);


		_userRepository.Delete(
			entity);

		await _userRepository.SaveChangesAsync(
			cancellationToken);
	}

	public async Task ChangePasswordAsync(int userId, string oldPassword, string newPassword, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
			?? throw new NotFoundException($"User {userId} not found");

		if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
			throw new ValidationException("Old password is incorrect");

		user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
		_userRepository.Update(user);
		await _userRepository.SaveChangesAsync(cancellationToken);
	}
}