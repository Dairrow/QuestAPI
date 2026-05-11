using API.Middleware;

namespace API.Extensions;

public static class MiddlewareExtensions
{
	public static IApplicationBuilder
		UseGlobalExceptionHandling(
		this IApplicationBuilder app)
	{
		app.UseMiddleware<
			ExceptionHandlingMiddleware>();

		return app;
	}
}