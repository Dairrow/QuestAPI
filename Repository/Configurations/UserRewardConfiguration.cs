using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class UserRewardConfiguration
	: IEntityTypeConfiguration<UserReward>
{
	public void Configure(
		EntityTypeBuilder<UserReward> builder)
	{
		builder.ToTable("user_rewards");


		builder.HasKey(x => x.Id);


		builder.HasOne(x => x.User)
			.WithMany(x => x.UserRewards)
			.HasForeignKey(x => x.UserId);


		builder.HasOne(x => x.Reward)
			.WithMany(x => x.UserRewards)
			.HasForeignKey(x => x.RewardId);
	}
}