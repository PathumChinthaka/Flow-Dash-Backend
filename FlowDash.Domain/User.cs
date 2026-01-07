namespace FlowDash.Domain
{
    public partial class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<UserRole> UserRolesCreatedBy { get; set; } = new List<UserRole>();
        public virtual ICollection<UserRole> UserRolesUpdatedBy { get; set; } = new List<UserRole>();
        public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
        public virtual ICollection<TeamMember> TeamMembersCreatedBy { get; set; } = new List<TeamMember>();
        public virtual ICollection<TeamMember> TeamMembersUpdatedBy { get; set; } = new List<TeamMember>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<Project> ProjectCreatedBy { get; set; } = new List<Project>();
        public virtual ICollection<Project> ProjectUpdatedBy { get; set; } = new List<Project>();
        public virtual ICollection<Task> TaskAssignee { get; set; } = new List<Task>();
        public virtual ICollection<Task> TaskCreatedBy { get; set; } = new List<Task>();
        public virtual ICollection<Task> TaskUpdatedBy { get; set; } = new List<Task>();
        public virtual ICollection<Team> TeamCreatedBy { get; set; } = new List<Team>();
        public virtual ICollection<Team> TeamUpdatedBy { get; set; } = new List<Team>();
        public virtual ICollection<TaskActivityLog> TaskChangedBy { get; set; } = new List<TaskActivityLog>();
        public virtual ICollection<TaskComment> TaskCommentCreatedBy { get; set; } = new List<TaskComment>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
