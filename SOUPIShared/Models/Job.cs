using SOUPIShared.Misc;
using System.ComponentModel.DataAnnotations;
using SOUPIShared.Attributes;


namespace SOUPIShared.Models
{
    public class Job
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
        public DateOnly StartDateTime { get; set; }

        [Required]
        public DateOnly EndDateTime { get; set; }

        [Required]
        [Range(0, 100)]
        public int Progress { get; set; }

        public DateTime CreationDateTime { get; set; } = default; 

        public JobStatus Status { get; set; } = JobStatus.New;

        [Required]
        public bool IsCompleted { get; set; }

        public DateTime? CompletedDateTime { get; set; } 

        public Guid? ParentJobId { get; set; }


        public virtual Project Project { get; set; } = default!; 
        public virtual TeamMember Creator { get; set; } = default!; 
        public virtual Job? ParentJob { get; set; }
        public virtual List<Job> ChildJobs { get; set; } = default!; 
        public virtual List<Assignment> Assignments { get; set; } = default!;
        public virtual List<JobSequence> NextJobSequences { get; set; } = default!; 
        public virtual List<JobSequence> PreviousJobSequences { get; set; } = default!; 
    }
}
