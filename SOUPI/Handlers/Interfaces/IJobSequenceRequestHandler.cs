using SOUPIShared.Dtos;


namespace SOUPI.Handlers.Interfaces
{
    public interface IJobSequenceRequestHandler
    {
        public Task<IEnumerable<JobSequenceDto>> GetByProjectId(Guid projectId); 

        public Task<JobSequenceDto> Create(Guid firstJobId, Guid secondJobId); 
        
        public Task Delete(Guid jobSequenceId); 
    }
}
