using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces 
{
    public interface IAssignmentRequestHandler
    {
        public Task<AssignmentDisplayDto> GetById(Guid assignmentId, CancellationToken ct = default);

        public Task<IEnumerable<AssignmentDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default); 

        public Task<IEnumerable<AssignmentDisplayDto>> GetByJobId(Guid jobId, CancellationToken ct = default); 

        public Task<IEnumerable<AssignmentDisplayDto>> GetByUserId(Guid userId, CancellationToken ct = default);

        public Task<AssignmentDto> Create(AssignmentDto newAssignmentDto, CancellationToken ct = default); 
        
        public Task<AssignmentDto> UpdateContent(AssignmentDto updatedAssignmentDto, CancellationToken ct = default); 
        
        public Task Delete(Guid assignmentId, CancellationToken ct = default); 
    }
}
