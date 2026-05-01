using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces
{
    public interface IJobSequenceRequestHandler
    {
        public Task<IEnumerable<JobSequenceDisplayDto>> GetByProjectId(Guid projectId); 

        public Task<JobSequenceDto> Create(Guid firstJobId, Guid secondJobId); 
        
        public Task Delete(Guid jobSequenceId); 
    }
}
