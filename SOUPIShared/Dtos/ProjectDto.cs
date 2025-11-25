using System.ComponentModel.DataAnnotations;
using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Поле названия обязательное")]
        [MaxLength(100, ErrorMessage = "Название слишком длинное (максимум 50 символов)")]
        [MinLength(1, ErrorMessage = "Название слишком короткое (минимум 1 символ)")]
        public string Name { get; set; } = null!;
        [MaxLength(255, ErrorMessage = "Описание слишком длинное (максимум 100 символов)")]
        public string? Description { get; set; }
        public string? GithubRepository { get; set; }

        [Required]
        public Guid CreatorId { get; set; }
        public DateTime CreationDateTime { get; set; } = DateTime.Now;
        public string? Image { get; set; }
    
        public ProjectDto(Project project)
        {
            Id = project.Id; 
            Name = project.Name; 
            Description = project.Description; 
            GithubRepository = project.GithubRepository; 
            CreatorId = project.CreatorId; 
            CreationDateTime = project.CreationDateTime; 
            Image = project.Image;
        }

        public ProjectDto() { }
    }
}
