using Microsoft.OpenApi.Models;

namespace API.Extensions;

public static class SwaggerExtensions
{
	public static IServiceCollection AddSwaggerDocumentation(
		this IServiceCollection services)
	{
		services.AddEndpointsApiExplorer();

		services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc(
				"v1",
				new OpenApiInfo
				{
					Title = "Quest API",
					Version = "v1"
				});
		});


		return services;
	}
}