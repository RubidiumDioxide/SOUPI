using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces
{
    public interface IProjectRequestHandler 
    {
        public Task<IEnumerable<ProjectDto>> GetByUserId(Guid userId); 
        
        public Task<ProjectDto?> GetById(Guid id);
        
        public Task<ProjectDto> Create(ProjectDto projectDto);
        
        public Task<ProjectDto> Update(ProjectDto updatedProjectDto);

        public Task<ProjectDto> UpdateCreator(ProjectDto updatedProjectDto); 
        
        public Task<ProjectDto> SetGitHubRepository(Guid projectId, string repositoryName); 

        public Task Delete(Guid id); 
    }
}
