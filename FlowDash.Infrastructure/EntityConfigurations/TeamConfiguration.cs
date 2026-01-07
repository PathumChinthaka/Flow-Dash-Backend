using FlowDash.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDash.Infrastructure.EntityConfigurations
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasKey(e => e.Id).HasName("Team_pkey");
            builder.ToTable("Team");

            builder.Property(e => e.Id).UseIdentityAlwaysColumn();
            builder.Property(e => e.Guid).HasColumnType("uuid");
            builder.Property(e => e.CreatedById).HasColumnName("CreatedBy");
            builder.Property(e => e.UpdatedById).HasColumnName("UpdatedBy");

            builder.Property(n => n.IsActive)
                   .HasDefaultValue(true);

            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.HasOne(t => t.CreatedBy)
                   .WithMany(u => u.TeamCreatedBy)
                   .HasForeignKey(t => t.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.TeamMembers)
                   .WithOne(tm => tm.Team)
                   .HasForeignKey(tm => tm.TeamId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => t.CreatedById);
            builder.HasIndex(t => t.Name);
        }
    }
}
