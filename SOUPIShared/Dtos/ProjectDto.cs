using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class ProjectDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? GithubRepository { get; set; }

        public int CreatorId { get; set; }

        public string? Image { get; set; }

        public ProjectDto(Project project)
        {
            Id = project.Id; 
            Name = project.Name; 
            GithubRepository = project.GithubRepository; 
            CreatorId = project.CreatorId; 
            Image = project.Image; 
        }

        public ProjectDto() { }
    }
}
