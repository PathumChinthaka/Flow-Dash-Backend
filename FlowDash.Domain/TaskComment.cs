namespace FlowDash.Domain
{
    public partial class TaskComment
    {
        public int Id { get; set; }
        public int TaskId { get;  set; }
        public string Comment { get; set; } = null!;
        public int CreatedById { get;  set; }
        public DateTime CreatedOn { get;  set; }
        public DateTime? UpdatedOn { get;  set; }
        public virtual Task Task { get; set; } = null!;
        public virtual User CreatedBy { get; set; } = null!;
    }
}
