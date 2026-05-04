using SOUPIShared.Attributes;
using SOUPIShared.Misc;
using SOUPIShared.Models;
using System.ComponentModel.DataAnnotations;


namespace SOUPIShared.Dtos.SOUPIDtos
{
    public class JobDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public Guid CreatorId { get; set; }

        [Required(ErrorMessage = "Поле названия обязательное")]
        [MaxLength(255, ErrorMessage = "Название слишком длинное (максимум 255 символов)")]
        [MinLength(1, ErrorMessage = "Название слишком короткое (минимум 1 символ)")]
        [ConsistsOfNumbersCyrillicLatin]
        public string Title { get; set; } = default!;

        [MaxLength(255, ErrorMessage = "Содержание задачи слишком длинное (максимум 255 символов)")]
        [ConsistsOfNumbersCyrillicLatin]
        public string? Body { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; } 
        
        [Required] 
        public DateTime EndDateTime { get; set; }

        [Required] 
        public int Progress { get; set; } 

        public DateTime CreationDateTime { get; set; } = DateTime.Now;

        public JobStatus Status { get; set; } = JobStatus.New;

        [Required]
        public bool IsCompleted { get; set; }

        public DateTime? CompletedDateTime { get; set; } 

        public Guid? ParentJobId { get; set; }

        public string? Dependencies { get; set; } = default!;

        public bool HasChildren { get; set; } = default!; 

        public JobDto(Job job)
        {
            Id = job.Id; 
            ProjectId = job.ProjectId; 
            CreatorId = job.CreatorId; 
            Title = job.Title; 
            Body = job.Body; 
            StartDateTime = job.StartDateTime; 
            EndDateTime = job.EndDateTime; 
            Progress = job.Progress; 
            CreationDateTime = job.CreationDateTime; 
            Status = job.Status; 
            IsCompleted = job.IsCompleted; 
            CompletedDateTime = job.CompletedDateTime; 
            ParentJobId = job.ParentJobId;
            
            if (job.PreviousJobSequences != null)
            {
                Dependencies = (job.PreviousJobSequences.Count == 0) ?
                   null :
                   string.Join(", ", job.PreviousJobSequences
                       .Select(js => js.FirstJobId.ToString()));
            }

            HasChildren = job.ChildJobs != null && job.ChildJobs.Count != 0; 
        }

        public JobDto(GanttJobDto ganttJobDto)
        {
            Id = Guid.Parse(ganttJobDto.id); 
            Title = ganttJobDto.name;
            StartDateTime = ganttJobDto.start;
            EndDateTime = ganttJobDto.end;
            Progress = ganttJobDto.progress;
            Dependencies = ganttJobDto.dependencies;
        }

        public JobDto(JobDisplayDto jobDisplayDto)
        {
            Id = jobDisplayDto.Id;
            ProjectId = jobDisplayDto.ProjectId;
            CreatorId = jobDisplayDto.CreatorId;
            Title = jobDisplayDto.Title;
            Body = jobDisplayDto.Body;
            StartDateTime = jobDisplayDto.StartDateTime;
            EndDateTime = jobDisplayDto.EndDateTime; 
            Progress = jobDisplayDto.Progress;
            CreationDateTime = jobDisplayDto.CreationDateTime;
            Status = jobDisplayDto.Status;
            IsCompleted = jobDisplayDto.IsCompleted; 
            CompletedDateTime = jobDisplayDto.CompletedDateTime; 
            ParentJobId = jobDisplayDto.ParentJobId;
            Dependencies = jobDisplayDto.Dependencies;
            HasChildren = jobDisplayDto.HasChildren; 
        }

        public JobDto() { }
    }
}
