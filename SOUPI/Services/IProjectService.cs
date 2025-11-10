using SOUPIShared.Dtos; 


namespace SOUPI.Services
{
    public interface IProjectService
    {
        public Task<IEnumerable<ProjectDto>> GetProjectsByLogin(string login);

        public Task<ProjectDto> CreateProject(ProjectDto projectDto); 
    }
}
