using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces
{
    public interface IJobSequenceRequestHandler
    {
        public Task<IEnumerable<JobSequenceDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default); 

        public Task<JobSequenceDto> Create(Guid firstJobId, Guid secondJobId, CancellationToken ct = default); 
        
        public Task Delete(Guid jobSequenceId, CancellationToken ct = default); 
    }
}
