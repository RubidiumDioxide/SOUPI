using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces 
{
    public interface IActivityRequestHandler
    {
        public Task<IEnumerable<ActivityDisplayDto>> GetByAssignmentId(Guid assignmentId, CancellationToken ct = default);

        public Task<IEnumerable<ActivityDisplayDto>> GetByTeamMemberId(Guid teamMemberId, CancellationToken ct = default);

        public Task<IEnumerable<ActivityDisplayDto>> GetByJobId(Guid jobId, CancellationToken ct = default); 

        public Task<IEnumerable<ActivityDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default);

        public Task<ActivityDto> Create(ActivityDto newActivity, CancellationToken ct = default);

        public Task<ActivityDto> UpdateContent(ActivityDto updatedActivity, CancellationToken ct = default); 

        public Task Delete(Guid activityId, CancellationToken ct = default);
    }
}
