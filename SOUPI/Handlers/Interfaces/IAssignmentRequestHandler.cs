using SOUPIShared.Dtos; 


namespace SOUPI.Handlers.Interfaces 
{
    public interface IAssignmentRequestHandler
    {
        public Task<IEnumerable<AssignmentDisplayDto>> GetByJobId(Guid jobId);

        public Task<IEnumerable<AssignmentDisplayDto>> GetByUserId(Guid userId);

        public Task<AssignmentDto> Create(AssignmentDto newAssignmentDto); 
        
        public Task<AssignmentDto> UpdateContent(AssignmentDto updatedAssignmentDto); 
        
        public Task Delete(Guid assignmentId); 
    }
}
