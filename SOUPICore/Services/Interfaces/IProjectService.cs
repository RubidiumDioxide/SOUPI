using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPICore.Services.Interfaces
{
    public interface IProjectService
    {
        public Task<IEnumerable<ProjectDisplayDto>> GetByUserId(Guid creatorId, CancellationToken ct = default);

        public Task<ProjectDisplayDto> GetById(Guid id, CancellationToken ct = default);

        public Task<ProjectDto> Create(ProjectDto projectDto, CancellationToken ct = default);

        public Task<ProjectDto> Update(ProjectDto changedProjectDto, CancellationToken ct = default); 

        public Task<ProjectDto> SetGitHubRepository(Guid projectId, string repositoryName, CancellationToken ct = default);

        public Task Delete(Guid Id, CancellationToken ct = default); 
    }
}
