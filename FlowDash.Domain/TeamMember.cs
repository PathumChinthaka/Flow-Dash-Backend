namespace FlowDash.Domain
{
    public partial class TeamMember
    {
        public int TeamId { get; set; }
        public int UserId { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public virtual Team Team { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual User CreatedBy { get; set; } = null!;
        public virtual User? UpdatedBy { get; set; }
    }
}
