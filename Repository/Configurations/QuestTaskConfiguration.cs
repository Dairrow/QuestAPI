using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class QuestTaskConfiguration
	: IEntityTypeConfiguration<QuestTask>
{
	public void Configure(
		EntityTypeBuilder<QuestTask> builder)
	{
		builder.ToTable("quest_tasks");


		builder.HasKey(x => x.Id);


		builder.Property(x => x.Title)
			.IsRequired();

		builder.Property(x => x.Order).IsRequired();


		builder.HasOne(x => x.Quest)
			.WithMany(x => x.Tasks)
			.HasForeignKey(x => x.QuestId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(x => new { x.QuestId, x.Order }).IsUnique();
	}
}