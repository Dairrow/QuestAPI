using System.Text.Json;
using API.Models;
using Services.Exceptions;

namespace API.Middleware;

public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;

	private readonly ILogger<
		ExceptionHandlingMiddleware> _logger;


	public ExceptionHandlingMiddleware(
		RequestDelegate next,
		ILogger<ExceptionHandlingMiddleware>
			logger)
	{
		_next = next;
		_logger = logger;
	}


	public async Task InvokeAsync(
		HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception exception)
		{
			await HandleExceptionAsync(
				context,
				exception);
		}
	}


	private async Task HandleExceptionAsync(
		HttpContext context,
		Exception exception)
	{
		_logger.LogError(
			exception,
			"Unhandled exception");


		context.Response.ContentType =
			"application/json";


		var response =
			new ApiErrorResponse();


		switch (exception)
		{
			case NotFoundException:

				context.Response.StatusCode =
					StatusCodes.Status404NotFound;

				response.StatusCode = 404;

				break;


			case ValidationException:

				context.Response.StatusCode =
					StatusCodes.Status400BadRequest;

				response.StatusCode = 400;

				break;


			case ForbiddenException:

				context.Response.StatusCode =
					StatusCodes.Status403Forbidden;

				response.StatusCode = 403;

				break;


			default:

				context.Response.StatusCode =
					StatusCodes
						.Status500InternalServerError;

				response.StatusCode = 500;

				break;
		}


		response.Message =
			exception.Message;


		var json =
			JsonSerializer.Serialize(
				response);


		await context.Response.WriteAsync(
			json);
	}
}