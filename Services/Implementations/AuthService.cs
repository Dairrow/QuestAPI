using Data.Enums;
using Services.Interfaces;
using Services.Models;

namespace Services.Implementations;

public class AuthService : IAuthService
{
	public Task<AuthResult> RegisterAsync(
		string username,
		string email,
		string password,
		UserRole role,
		CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}


	public Task<AuthResult> LoginAsync(
		string email,
		string password,
		CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}