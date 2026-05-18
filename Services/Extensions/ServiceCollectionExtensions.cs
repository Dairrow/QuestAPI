using Microsoft.Extensions.DependencyInjection;
using Services.Implementations;
using Services.Interfaces;

namespace Services.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBusinessServices(
		this IServiceCollection services)
	{
		services.AddScoped<IAuthService, AuthService>();

		services.AddScoped<IUserService, UserService>();

		services.AddScoped<IQuestService, QuestService>();

		services.AddScoped<IRewardService, RewardService>();

		services.AddScoped<IQuestTaskService, QuestTaskService>();

		services.AddScoped<ITokenService, TokenService>();

		services.AddScoped<IUserQuestService, UserQuestService>();

		services.AddScoped<IInventoryService, InventoryService>();

		return services;
	}

	public static IServiceCollection AddFileStorageService(this IServiceCollection services, string? webRootPath = null)
	{
		services.AddSingleton<IFileStorageService>(new FileStorageService(webRootPath));
		return services;
	}
}