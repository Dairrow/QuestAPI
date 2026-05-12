using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Auth;

public class RefreshRequest
{
	[Required]
	public string RefreshToken { get; set; } = string.Empty;
}