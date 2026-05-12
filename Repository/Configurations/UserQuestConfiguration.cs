using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class UserQuestConfiguration
	: IEntityTypeConfiguration<UserQuest>
{
	public void Configure(
		EntityTypeBuilder<UserQuest> builder)
	{
		builder.ToTable("user_quests");


		builder.HasKey(x => x.Id);


		builder.HasOne(x => x.User)
			.WithMany(x => x.UserQuests)
			.HasForeignKey(x => x.UserId);


		builder.HasOne(x => x.Quest)
			.WithMany(x => x.UserQuests)
			.HasForeignKey(x => x.QuestId);


		builder.HasIndex(x =>
			new { x.UserId, x.QuestId })
			.IsUnique();

		builder.Property(x => x.LastCompletedTaskOrder).IsRequired(false);
	}
}