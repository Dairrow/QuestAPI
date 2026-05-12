using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace API.DTOs.Requests.Users;

public class UpdateUserRequest
{
	[Required]
	[MinLength(3)]
	[MaxLength(50)]
	public string Username { get; set; } = string.Empty;

	[Required]
	[EmailAddress]
	public string Email { get; set; } = string.Empty;
}