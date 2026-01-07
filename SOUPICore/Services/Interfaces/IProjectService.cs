using SOUPIShared.Dtos;


namespace SOUPICore.Services.Interfaces
{
    public interface IProjectService
    {
        public Task<IEnumerable<ProjectDto>> GetByCreatorId(Guid creatorId);
 
        public Task<ProjectDto> GetById(Guid id);

        public Task<ProjectDto> Create(ProjectDto projectDto);

        public Task<ProjectDto> Update(ProjectDto changedProjectDto);

        public Task Delete(Guid Id); 
    }
}
