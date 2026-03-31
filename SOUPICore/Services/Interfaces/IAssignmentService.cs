

using SOUPIShared.Dtos;

namespace SOUPICore.Services.Interfaces
{
    public interface IAssignmentService
    {
        public Task<IEnumerable<AssignmentDto>> GetByJobId(Guid jobId); 

        public Task<AssignmentDto> Create(AssignmentDto newAssignmentDto);

        public Task Delete(Guid assignmentId);
    }
}
