namespace FlowDash.Domain
{
    public partial class Task
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int ProjectId { get; set; }
        public int? AssigneeId { get; set; }
        public DateTime DueDate { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual User? Assignee { get; set; }
        public virtual User CreatedBy { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
        public virtual User? UpdatedBy { get; set; }
        public virtual ICollection<TaskActivityLog> TaskActivityLogs { get; set; } = new List<TaskActivityLog>();
        public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();
    }
}
