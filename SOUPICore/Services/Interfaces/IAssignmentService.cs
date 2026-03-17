

using SOUPIShared.Dtos;

namespace SOUPICore.Services.Interfaces
{
    public interface IAssignmentService
    {
        public Task CreateAssignment(AssignmentDto newAssignmentDto);

        public Task DeleteAssignment(Guid assignmentId);
    }
}
