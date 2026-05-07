using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPICore.Services.Interfaces
{
    public interface IJobService
    {
        public Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectId(Guid projectId, CancellationToken ct = default);

        public Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectIdParentId(Guid projectId, Guid? parentJobId, CancellationToken ct = default);

        public Task<IEnumerable<JobDisplayDto>> GetDisplayByUserId(Guid userId, CancellationToken ct = default);

        public Task<JobDisplayDto> GetDisplayById(Guid jobId, CancellationToken ct = default);

        public Task<IEnumerable<JobDto>> GetByProjectId(Guid projectId, CancellationToken ct = default);

        public Task<IEnumerable<JobDto>> GetByProjectIdParentId(Guid projectId, Guid? parentJobId, CancellationToken ct = default);

        public Task<JobDto> GetById(Guid jobId, CancellationToken ct = default); 

        public Task<JobDto> Create(JobDto newJob, CancellationToken ct = default);

        public Task<JobDto> UpdateContent(JobDto updatedJob, CancellationToken ct = default); 

        public Task<JobDto> UpdateParent(Guid jobId, Guid? newParentJobId, CancellationToken ct = default);

        public Task Delete (Guid jobId, bool preserveChildren, CancellationToken ct = default);
    }
}
