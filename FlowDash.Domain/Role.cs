namespace FlowDash.Domain
{
    public partial class Role
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
