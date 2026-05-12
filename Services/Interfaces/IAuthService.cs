using Data.Enums;
using Services.Models;

namespace Services.Interfaces;

public interface IAuthService
{
	Task<AuthResult> RegisterAsync(
		string username,
		string email,
		string password,
		UserRole role,
		CancellationToken cancellationToken = default);

	Task<AuthResult> LoginAsync(
		string email,
		string password,
		CancellationToken cancellationToken = default);

	Task<AuthResult> 
		RefreshAsync(string refreshToken, 
		CancellationToken cancellationToken = default);

	Task RevokeUserTokensAsync(
		int userId,
		CancellationToken cancellationToken = default);
}