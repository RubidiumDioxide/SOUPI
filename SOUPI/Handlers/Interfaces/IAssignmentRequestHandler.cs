using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces 
{
    public interface IAssignmentRequestHandler
    {
        public Task<AssignmentDisplayDto> GetById(Guid assignmentId);

        public Task<IEnumerable<AssignmentDisplayDto>> GetByProjectId(Guid projectId); 

        public Task<IEnumerable<AssignmentDisplayDto>> GetByJobId(Guid jobId);

        public Task<IEnumerable<AssignmentDisplayDto>> GetByUserId(Guid userId);

        public Task<AssignmentDto> Create(AssignmentDto newAssignmentDto); 
        
        public Task<AssignmentDto> UpdateContent(AssignmentDto updatedAssignmentDto); 
        
        public Task Delete(Guid assignmentId); 
    }
}
