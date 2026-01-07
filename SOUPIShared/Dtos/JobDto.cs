using SOUPIShared.Models;
using System.ComponentModel.DataAnnotations;


namespace SOUPIShared.Dtos
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
        public string Title { get; set; } = default!;

        [MaxLength(255, ErrorMessage = "Содержание задачи слишком длинное (максимум 255 символов)")]
        public string? Body { get; set; }

        public DateTime? Deadline { get; set; }

        public DateTime? CreationDateTime { get; set; } = DateTime.Now;

        public JobStatus Status { get; set; } = JobStatus.New;

        public Guid? ParentJobId { get; set; }


        public JobDto(Job job)
        {
            Id = job.Id; 
            ProjectId = job.ProjectId; 
            CreatorId = job.CreatorId; 
            Title = job.Title; 
            Body = job.Body; 
            Deadline = job.Deadline; 
            CreationDateTime = job.CreationDateTime; 
            Status = job.Status; 
            ParentJobId = job.ParentJobId; 
        }

        public JobDto() { }
    }
}
