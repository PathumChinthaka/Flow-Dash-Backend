namespace FlowDash.Domain.Entities
{
    public partial class TaskActivityLog
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public int TaskId { get; set; }
        public string Action { get; set; } = null!;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public int ChangedById { get; set; }
        public DateTime ChangedOn { get; set; }
        public virtual Task Task { get; set; } = null!;
        public virtual User ChangedBy { get; set; } = null!;
    }
}
