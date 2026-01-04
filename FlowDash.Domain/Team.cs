namespace FlowDash.Domain
{
    public partial class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual User CreatedBy { get; set; } = null!;
        public virtual User? UpdatedBy { get; set; }
        public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    }
}
