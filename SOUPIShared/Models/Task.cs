namespace SOUPIShared.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int CreatorId { get; set; }

    public int AssigneeId { get; set; }

    public int? ParentTaskId { get; set; }

    public string Status { get; set; } = null!;

    public TimeOnly? TimeCost { get; set; }

    public int ProjectId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual User Assignee { get; set; } = null!;

    public virtual User Creator { get; set; } = null!;

    public virtual ICollection<Task> InverseParentTask { get; set; } = new List<Task>();

    public virtual Task? ParentTask { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<Task> PreceedingTasks { get; set; } = new List<Task>();

    public virtual ICollection<Task> SubsequentTasks { get; set; } = new List<Task>();
}
