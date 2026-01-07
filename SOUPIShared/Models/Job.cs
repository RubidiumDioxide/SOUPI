using System.ComponentModel.DataAnnotations; 


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
        [MaxLength(100, ErrorMessage = "Название слишком длинное (максимум 100 символов)")]
        [MinLength(1, ErrorMessage = "Название слишком короткое (минимум 1 символ)")]
        public string Title { get; set; } = default!;

        [MaxLength(255, ErrorMessage = "Содержание задачи слишком длинное (максимум 255 символов)")]
        [MinLength(1, ErrorMessage = "Название слишком короткое (минимум 1 символ)")]
        public string? Body { get; set; } 

        public DateTime? Deadline { get; set; }

        public DateTime? CreationDateTime { get; set; } = DateTime.Now;

        public JobStatus Status { get; set; } = JobStatus.New; 
 
        public Guid? ParentJobId { get; set; }


        public virtual Project Project { get; set; } = default!; 
        public virtual User Creator { get; set; } = default!; 
        public virtual Job? ParentJob { get; set; }
        public virtual List<Job> ChildJobs { get; set; } = default!;  
    }
}
