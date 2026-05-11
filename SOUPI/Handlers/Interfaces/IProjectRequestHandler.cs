using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces
{
    public interface IProjectRequestHandler 
    {
        public Task<IEnumerable<ProjectDisplayDto>> GetByUserId(Guid userId, CancellationToken ct = default); 
        
        public Task<ProjectDisplayDto?> GetById(Guid id, CancellationToken ct = default);
        
        public Task<ProjectDto> Create(ProjectDto projectDto, CancellationToken ct = default);
        
        public Task<ProjectDto> Update(ProjectDto updatedProjectDto, CancellationToken ct = default);
        
        public Task<ProjectDto> SetGitHubRepository(Guid projectId, string repositoryName, CancellationToken ct = default); 

        public Task Delete(Guid id, CancellationToken ct = default); 
    }
}
