using Data.Enums;

namespace API.DTOs.Responses.Users;

public class UserResponse
{
	public int Id { get; set; }

	public string Username { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;

	public UserRole Role { get; set; }

	public DateTime CreatedAt { get; set; }
}