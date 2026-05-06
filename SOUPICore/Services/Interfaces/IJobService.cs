using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPICore.Services.Interfaces
{
    public interface IJobService
    {
        public Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectId(Guid projectId);

        public Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectIdParentId(Guid projectId, Guid? parentJobId);

        public Task<IEnumerable<JobDisplayDto>> GetDisplayByUserId(Guid userId);

        public Task<JobDisplayDto> GetDisplayById(Guid jobId);

        public Task<IEnumerable<JobDto>> GetByProjectId(Guid projectId);
        
        public Task<IEnumerable<JobDto>> GetByProjectIdParentId(Guid projectId, Guid? parentJobId);

        public Task<JobDto> GetById(Guid jobId); 

        public Task<JobDto> Create(JobDto newJob);

        public Task<JobDto> UpdateContent(JobDto updatedJob); 
        
        public Task<JobDto> UpdateParent(Guid jobId, Guid? newParentJobId);

        public Task Delete (Guid jobId, bool preserveChildren);
    }
}
