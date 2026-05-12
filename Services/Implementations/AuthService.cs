using Data.Entities;
using Data.Enums;
using Repository.Interfaces;
using Services.Exceptions;
using Services.Interfaces;
using Services.Models;

namespace Services.Implementations;

public class AuthService
	: IAuthService
{
	private readonly IUserRepository
		_userRepository;

	private readonly
		IRefreshTokenRepository
		_refreshRepository;

	private readonly ITokenService
		_tokenService;


	public AuthService(
		IUserRepository userRepository,
		IRefreshTokenRepository
			refreshRepository,
		ITokenService tokenService)
	{
		_userRepository =
			userRepository;

		_refreshRepository =
			refreshRepository;

		_tokenService =
			tokenService;
	}


	public async Task<AuthResult>
		RegisterAsync(
		string username,
		string email,
		string password,
		UserRole role,
		CancellationToken cancellationToken =
			default)
	{
		if (await _userRepository
			.GetByEmailAsync(
				email,
				cancellationToken)
			is not null)
		{
			throw new ValidationException(
				"Email already exists");
		}


		var user = new User
		{
			Username = username,

			Email = email,

			PasswordHash =
				BCrypt.Net.BCrypt
					.HashPassword(
						password),

			Role = role
		};


		await _userRepository
			.AddAsync(
				user,
				cancellationToken);

		await _userRepository
			.SaveChangesAsync(
				cancellationToken);


		return await CreateAuthResult(
			user,
			cancellationToken);
	}


	public async Task<AuthResult>
		LoginAsync(
		string email,
		string password,
		CancellationToken cancellationToken =
			default)
	{
		var user =
			await _userRepository
				.GetByEmailAsync(
					email,
					cancellationToken)
			?? throw new ValidationException(
				"Invalid credentials");


		if (!BCrypt.Net.BCrypt
			.Verify(
				password,
				user.PasswordHash))
		{
			throw new ValidationException(
				"Invalid credentials");
		}

	await _refreshRepository
			.RevokeAllForUserAsync(
			user.Id, 
			cancellationToken);

	return await CreateAuthResult(
			user,
			cancellationToken);
	}

	public async Task<AuthResult> 
		RefreshAsync(string refreshToken, 
		CancellationToken cancellationToken = default)
	{
	var existing = await _refreshRepository
			.GetByTokenAsync(
			refreshToken, 
			cancellationToken);

	if (existing == null || existing.IsRevoked || existing.ExpiresAt <= DateTime.UtcNow)
	throw new ValidationException("Invalid or expired refresh token");

	existing.IsRevoked = true;
	_refreshRepository
			.Update(existing);

	await _refreshRepository
			.SaveChangesAsync(cancellationToken);

	var user = await _userRepository
			.GetByIdAsync(
			existing.UserId, 
			cancellationToken);

	if (user == null)
	throw new NotFoundException("User not found");

	return await CreateAuthResult(user, cancellationToken);
	}


	private async Task<AuthResult>
		CreateAuthResult(
		User user,
		CancellationToken cancellationToken)
	{
		var accessToken =
			_tokenService
				.GenerateAccessToken(
					user);

		var refreshToken =
			_tokenService
				.GenerateRefreshToken();


		await _refreshRepository
			.AddAsync(
				new RefreshToken
				{
					Token =
						refreshToken,

					UserId =
						user.Id,

					ExpiresAt =
						DateTime.UtcNow
							.AddDays(30)
				},
				cancellationToken);


		await _refreshRepository
			.SaveChangesAsync(
				cancellationToken);


		return new AuthResult
		{
			UserId = user.Id,

			Username =
				user.Username,

			AccessToken =
				accessToken,

			RefreshToken =
				refreshToken
		};
	}

	public async Task RevokeUserTokensAsync(
		int userId,
		CancellationToken cancellationToken = default)
	{
	var user = await _userRepository
	.GetByIdAsync(userId, cancellationToken);

	if (user == null)
	throw new NotFoundException("User not found");

	await _refreshRepository
	.RevokeAllForUserAsync(
	userId,
	cancellationToken);
	}
}