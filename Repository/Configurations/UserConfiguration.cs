using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class UserConfiguration
	: IEntityTypeConfiguration<User>
{
	public void Configure(
		EntityTypeBuilder<User> builder)
	{
		builder.ToTable("users");


		builder.HasKey(x => x.Id);


		builder.Property(x => x.Username)
			.IsRequired()
			.HasMaxLength(50);


		builder.Property(x => x.Email)
			.IsRequired()
			.HasMaxLength(100);


		builder.Property(x => x.PasswordHash)
			.IsRequired();


		builder.HasIndex(x => x.Email)
			.IsUnique();
	}
}