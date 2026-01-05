using FlowDash.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDash.Infrastructure.EntityConfigurations
{
    public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
    {
        public void Configure(EntityTypeBuilder<TeamMember> builder)
        {
            builder.HasKey(tm => new { tm.TeamId, tm.UserId });
            builder.Property(e => e.CreatedById).HasColumnName("CreatedBy");
            builder.Property(e => e.UpdatedById).HasColumnName("UpdatedBy");

            builder.HasOne(tm => tm.Team)
                   .WithMany(t => t.TeamMembers)
                   .HasForeignKey(tm => tm.TeamId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tm => tm.User)
                   .WithMany(u => u.TeamMembers)
                   .HasForeignKey(tm => tm.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tm => tm.CreatedBy)
                   .WithMany(u => u.TeamMembersCreatedBy)
                   .HasForeignKey(tm => tm.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tm => tm.UpdatedBy)
                   .WithMany(u => u.TeamMembersUpdatedBy)
                   .HasForeignKey(tm => tm.UpdatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(tm => tm.CreatedById);
            builder.HasIndex(tm => tm.UpdatedById);
        }
    }
}
