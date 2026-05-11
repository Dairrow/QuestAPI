using Repository.Implementations;
using Repository.Interfaces;

namespace API.Extensions;

public static class RepositoryExtensions
{
	public static IServiceCollection AddRepositories(
		this IServiceCollection services)
	{
		services.AddScoped<IUserRepository, UserRepository>();

		services.AddScoped<IQuestRepository, QuestRepository>();

		services.AddScoped<IRewardRepository, RewardRepository>();


		return services;
	}
}