using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces 
{
    public interface IJobRequestHandler
    {
        public Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectId(Guid projectId, CancellationToken ct = default);

        public Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectIdParentId(Guid projectId, Guid? parentJobId, CancellationToken ct = default);

        public Task<IEnumerable<JobDisplayDto>> GetDisplayByUserId(Guid userId, CancellationToken ct = default);

        public Task<JobDisplayDto> GetDisplayById(Guid jobId, CancellationToken ct = default);

        public Task<IEnumerable<JobDto>> GetByProjectId(Guid projectId, CancellationToken ct = default); 

        public Task<IEnumerable<JobDto>> GetByProjectIdParentId(Guid projectId, Guid? parentJobId, CancellationToken ct = default);

        public Task<JobDto> GetById(Guid jobId, CancellationToken ct = default);

        public Task<JobDto> Create(JobDto newJobDto, CancellationToken ct = default); 
        
        public Task<JobDto> UpdateContent(JobDto updatedJobDto, CancellationToken ct = default); 
        
        public Task<JobDto> UpdateParent(Guid jobId, Guid? newParentJobId, CancellationToken ct = default); 
        
        public Task Delete(Guid jobId, CancellationToken ct = default); 
    }
}
