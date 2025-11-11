using SOUPIShared.Dtos; 


namespace SOUPI.Services
{
    public interface IProjectService
    {
        public Task<IEnumerable<ProjectDto>> GetByLogin(string login);

        public Task<ProjectDto> Create(ProjectDto projectDto); 
    }
}
