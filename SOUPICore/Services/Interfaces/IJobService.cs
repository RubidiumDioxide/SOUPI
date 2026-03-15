using SOUPIShared.Dtos;


namespace SOUPICore.Services.Interfaces
{
    public interface IJobService
    {
        public Task<IEnumerable<JobDto>> GetByProjectId(Guid projectId);

        public Task<JobDto> GetById(Guid jobId); 

        public Task<JobDto> Create(JobDto newJob);

        public Task<JobDto> UpdateContent(JobDto updatedJob); 
        
        public Task<JobDto> UpdateParent(Guid jobId, Guid? newParentId);

        public Task CreateLink(Guid firstJobId, Guid secondJobId); 

        public Task DeleteLink(Guid jobSequenceId); 

        public Task Delete (Guid jobId, bool preserveChildren); 
    }
}
