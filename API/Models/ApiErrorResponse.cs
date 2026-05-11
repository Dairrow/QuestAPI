namespace API.Models;

public class ApiErrorResponse
{
	public bool Success => false;

	public int StatusCode { get; set; }

	public string Message { get; set; }
		= string.Empty;
}