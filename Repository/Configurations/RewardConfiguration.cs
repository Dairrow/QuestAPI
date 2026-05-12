using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class RewardConfiguration
	: IEntityTypeConfiguration<Reward>
{
	public void Configure(
		EntityTypeBuilder<Reward> builder)
	{
		builder.ToTable("rewards");


		builder.HasKey(x => x.Id);


		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(x => x.ImagePath)
			.HasMaxLength(500)
			.IsRequired(false);
	}
}