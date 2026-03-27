using SOUPIShared.Dtos;


namespace SOUPICore.Services.Interfaces
{
    public interface IJobSequenceService
    {
        public Task<JobSequenceDto> Create(Guid firstJobId, Guid secondJobId); 

        public Task Delete(Guid jobSequenceId);
    }
}
