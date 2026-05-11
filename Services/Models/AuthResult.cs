namespace Services.Models;

public class AuthResult
{
	public int UserId { get; set; }

	public string Username { get; set; } = string.Empty;

	public string AccessToken { get; set; } = string.Empty;

	public string RefreshToken { get; set; } = string.Empty;
}