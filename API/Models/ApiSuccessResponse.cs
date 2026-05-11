namespace API.Models;

public class ApiSuccessResponse<T>
{
	public bool Success => true;

	public T Data { get; set; }


	public ApiSuccessResponse(
		T data)
	{
		Data = data;
	}
}