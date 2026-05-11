using Data.Entities;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;
using Services.Exceptions;
using Services.Interfaces;

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
}