using SOUPIShared.Dtos;
using SOUPIShared.Models;


namespace SOUPICore.Services.Interfaces
{
    public interface IJobService
    {
        // --- JOB ---
        public Task<IEnumerable<JobDto>> GetByProjectId(Guid projectId);

        public Task<JobDto> GetById(Guid jobId); 

        public Task<JobDto> Create(JobDto newJob);

        public Task<JobDto> UpdateContent(JobDto updatedJob); 
        
        public Task<JobDto> UpdateParent(Guid jobId, Guid? newParentId);

        public Task Delete (Guid jobId, bool preserveChildren);


        // --- JOBSEQUENCE --- 

        // --- ASSIGNMENT --- 
    }
}
