using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Configurations;

namespace Repository.Context;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
	{
	}

	public DbSet<User> Users => Set<User>();

	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

	public DbSet<Quest> Quests => Set<Quest>();

	public DbSet<QuestTask> QuestTasks => Set<QuestTask>();

	public DbSet<Reward> Rewards => Set<Reward>();

	public DbSet<UserQuest> UserQuests => Set<UserQuest>();

	public DbSet<UserReward> UserRewards => Set<UserReward>();


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);


		modelBuilder.ApplyConfiguration(
			new UserConfiguration());

		modelBuilder.ApplyConfiguration(
			new RefreshTokenConfiguration());

		modelBuilder.ApplyConfiguration(
			new QuestConfiguration());

		modelBuilder.ApplyConfiguration(
			new QuestTaskConfiguration());

		modelBuilder.ApplyConfiguration(
			new RewardConfiguration());

		modelBuilder.ApplyConfiguration(
			new UserQuestConfiguration());

		modelBuilder.ApplyConfiguration(
			new UserRewardConfiguration());
	}
}