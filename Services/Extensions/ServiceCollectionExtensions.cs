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

		return services;
	}
}