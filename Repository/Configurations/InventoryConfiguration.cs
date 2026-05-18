using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class InventoryConfiguration
	: IEntityTypeConfiguration<Inventory>
{
	public void Configure(
		EntityTypeBuilder<Inventory> builder)
	{
		builder.ToTable("user_rewards");


		builder.HasKey(x => x.Id);

		builder.Property(x => x.IsClaimed).IsRequired();

		builder.HasOne(x => x.User)
			.WithMany(x => x.UserRewards)
			.HasForeignKey(x => x.UserId);


		builder.HasOne(x => x.Reward)
			.WithMany(x => x.UserRewards)
			.HasForeignKey(x => x.RewardId);

		builder.HasIndex(x => new { x.UserId, x.RewardId }).IsUnique();
	}
}