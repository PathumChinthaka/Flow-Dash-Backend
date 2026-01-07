namespace FlowDash.Domain.Entities
{
    public partial class TeamMember
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public int TeamId { get; set; }
        public int UserId { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public virtual Team Team { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual User CreatedBy { get; set; } = null!;
        public virtual User? UpdatedBy { get; set; }
    }
}
