using FlowDash.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDash.Infrastructure.EntityConfigurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(e => e.Id).HasName("Notification_pkey");
            builder.ToTable("Notification");

            builder.Property(e => e.Id).UseIdentityAlwaysColumn();
            builder.Property(e => e.Guid).HasColumnType("uuid");

            builder.Property(n => n.UserId)
                   .IsRequired();

            builder.Property(n => n.Message)
                   .IsRequired()
                   .HasMaxLength(1000); 

            builder.Property(n => n.IsRead)
                   .HasDefaultValue(false);

            builder.HasOne(n => n.User)
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
