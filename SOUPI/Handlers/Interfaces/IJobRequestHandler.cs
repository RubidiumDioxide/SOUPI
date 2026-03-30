using SOUPIShared.Dtos;

namespace SOUPI.Handlers.Interfaces
{
    public interface IJobRequestHandler
    {
        public Task<JobDto> GetById(Guid jobId); 
        
        public Task<IEnumerable<JobDto>> GetByProjectId(Guid projectId); 
        
        public Task<JobDto> Create(JobDto newJobDto); 
        
        public Task<JobDto> UpdateContent(JobDto updatedJobDto); 
        
        public Task<JobDto> UpdateParent(JobDto updatedJobDto); 
        
        public Task Delete(Guid jobId); 
    }
}
