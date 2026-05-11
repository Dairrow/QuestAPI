using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class QuestConfiguration
	: IEntityTypeConfiguration<Quest>
{
	public void Configure(
		EntityTypeBuilder<Quest> builder)
	{
		builder.ToTable("quests");


		builder.HasKey(x => x.Id);


		builder.Property(x => x.Title)
			.IsRequired()
			.HasMaxLength(100);


		builder.Property(x => x.Description)
			.IsRequired();
	}
}