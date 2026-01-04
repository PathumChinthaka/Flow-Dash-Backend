using FlowDash.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDash.Infrastructure.EntityConfigurations
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasKey(t => t.Id);

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
