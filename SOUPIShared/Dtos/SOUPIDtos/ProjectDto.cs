using SOUPIShared.Models;
using System.ComponentModel.DataAnnotations;
using SOUPIShared.Attributes;


namespace SOUPIShared.Dtos.SOUPIDtos
{
    public class ProjectDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Поле названия обязательное")]
        [MaxLength(100, ErrorMessage = "Название слишком длинное (максимум 50 символов)")]
        [MinLength(1, ErrorMessage = "Название слишком короткое (минимум 1 символ)")]
        [ConsistsOfNumbersCyrillicLatin]
        public string Title { get; set; } = null!;
        
        [MaxLength(255, ErrorMessage = "Описание слишком длинное (максимум 100 символов)")]
        [ConsistsOfNumbersCyrillicLatin] 
        public string? Description { get; set; }

        [ValidGitHubRepositoryName]
        public string? GithubRepository { get; set; }
        
        [Required]
        public Guid CreatorId { get; set; }
        
        [Required]
        public DateTime CreationDateTime { get; set; } 


        public ProjectDto(Project project)
        {
            Id = project.Id; 
            Title = project.Title; 
            Description = project.Description; 
            GithubRepository = project.GithubRepository; 
            CreatorId = project.CreatorId; 
            CreationDateTime = project.CreationDateTime;  
        }

        public ProjectDto() { }
    }
}
