using System.ComponentModel.DataAnnotations;
using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? GithubRepository { get; set; }
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
