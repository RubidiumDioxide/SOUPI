using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces 
{
    public interface IJobRequestHandler
    {
        public Task<JobDisplayDto> GetDisplayById(Guid jobId);

        public Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectId(Guid projectId);

        public Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectIdParentId(Guid projectId, Guid? parentJobId);

        public Task<JobDto> GetById(Guid jobId); 

        public Task<IEnumerable<JobDto>> GetByProjectId(Guid projectId); 
        
        public Task<IEnumerable<JobDto>> GetByProjectIdParentId(Guid projectId, Guid? parentJobId); 
        
        public Task<JobDto> Create(JobDto newJobDto); 
        
        public Task<JobDto> UpdateContent(JobDto updatedJobDto); 
        
        public Task<JobDto> UpdateParent(Guid jobId, Guid? newParentJobId); 
        
        public Task Delete(Guid jobId, bool preserveChildren); 
    }
}
