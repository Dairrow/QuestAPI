using Data.Entities;

namespace Services.Interfaces;

public interface ITokenService
{
	string GenerateAccessToken(
		User user);

	string GenerateRefreshToken();
}