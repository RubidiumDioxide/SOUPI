using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPICore.Services.Interfaces
{
    public interface IProjectService
    {
        public Task<IEnumerable<ProjectDisplayDto>> GetByUserId(Guid creatorId);
 
        public Task<ProjectDisplayDto> GetById(Guid id);

        public Task<ProjectDto> Create(ProjectDto projectDto);

        public Task<ProjectDto> Update(ProjectDto changedProjectDto);

        public Task<ProjectDto> UpdateCreator(ProjectDto updatedProjectDto);

        public Task<ProjectDto> SetGitHubRepository(Guid projectId, string repositoryName);

        public Task Delete(Guid Id); 
    }
}
