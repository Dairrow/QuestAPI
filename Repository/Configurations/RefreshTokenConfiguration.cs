using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class RefreshTokenConfiguration
	: IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(
		EntityTypeBuilder<RefreshToken> builder)
	{
		builder.ToTable("refresh_tokens");


		builder.HasKey(x => x.Id);


		builder.Property(x => x.Token)
			.IsRequired();


		builder.HasOne(x => x.User)
			.WithMany()
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}