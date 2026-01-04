namespace FlowDash.Domain
{
    public partial class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    }
}
