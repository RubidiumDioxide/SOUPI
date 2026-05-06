using SOUPIShared.Models;
using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPIShared.Extensions
{
    public static class ProjectDtoExtensions
    {
        public static void CopyContentProperties(this ProjectDto firstProjectDto, ProjectDto secondProjectDto)
        {
            firstProjectDto.Title = secondProjectDto.Title;
            firstProjectDto.Description = secondProjectDto.Description;
        }

        public static void CopyContentProperties(this ProjectDto projectDto, Project project)
        {
            projectDto.Title = project.Title;
            projectDto.Description = project.Description;
        }

        public static void CopyContentProperties(this ProjectDto projectDto, ProjectDisplayDto projectDisplayDto)
        {
            projectDto.Title = projectDisplayDto.Title;
            projectDto.Description = projectDisplayDto.Description;
        }
    }
}
