using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Users;

public class ChangePasswordRequest
{
	[Required]
	public string OldPassword { get; set; } = string.Empty;

	[Required]
	[MinLength(8)]
	public string NewPassword { get; set; } = string.Empty;
}