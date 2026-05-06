using SOUPIShared.Models;
using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPIShared.Extensions
{
    public static class ProjectDisplayDtoExtensions
    {
        public static void CopyContentProperties(this ProjectDisplayDto projectDisplayDto, ProjectDto projectDto)
        {
            projectDisplayDto.Title = projectDto.Title;
            projectDisplayDto.Description = projectDto.Description;
        }

        public static void CopyContentProperties(this ProjectDisplayDto projectDisplayDto, Project project)
        {
            projectDisplayDto.Title = project.Title;
            projectDisplayDto.Description = project.Description;
        }
    }
}
