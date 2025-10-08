using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class TaskDto
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

        public TaskDto(SOUPIShared.Models.Task task)
        {
            Id = task.Id;
            Name = task.Name; 
            Description = task.Description; 
            CreatorId = task.CreatorId; 
            AssigneeId = task.AssigneeId; 
            ParentTaskId = task.ParentTaskId; 
            Status = task.Status; 
            TimeCost = task.TimeCost; 
            ProjectId = task.ProjectId; 
            StartTime = task.StartTime; 
            EndTime = task.EndTime; 
        }

        public TaskDto() { }
    }
}
