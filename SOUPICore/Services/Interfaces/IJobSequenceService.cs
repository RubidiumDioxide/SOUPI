

using SOUPIShared.Dtos;

namespace SOUPICore.Services.Interfaces
{
    public interface IJobSequenceService
    {
        public Task CreateSequence(JobSequenceDto newJobSequenceDto);

        public Task DeleteSequence(Guid jobSequenceId);
    }
}
