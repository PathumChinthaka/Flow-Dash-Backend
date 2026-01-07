using FlowDash.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDash.Infrastructure.EntityConfigurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshToken");

            builder.HasKey(e => e.Id)
                   .HasName("RefreshToken_pkey");

            builder.Property(e => e.Id)
                   .UseIdentityAlwaysColumn();

            builder.Property(e => e.Token)
                   .IsRequired()
                   .HasMaxLength(512);

            builder.Property(e => e.ExpiresOn)
                   .IsRequired();

            builder.Property(e => e.IsRevoked)
                   .HasDefaultValue(false);

            builder.HasOne(rt => rt.User)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(rt => rt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.Token)
                   .IsUnique();
        }
    }
}
