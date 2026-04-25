using SOUPIShared.Dtos; 


namespace SOUPI.Handlers.Interfaces 
{
    public interface IActivityRequestHandler
    {
        public Task<IEnumerable<ActivityDisplayDto>> GetByAssignmentId(Guid assignmentId);

        public Task<IEnumerable<ActivityDisplayDto>> GetByTeamMemberId(Guid teamMemberId);

        public Task<IEnumerable<ActivityDisplayDto>> GetByJobId(Guid jobId);

        public Task<IEnumerable<ActivityDisplayDto>> GetByProjectId(Guid projectId);

        public Task<ActivityDto> Create(ActivityDto newActivity);

        public Task<ActivityDto> UpdateContent(ActivityDto updatedActivity);

        public Task Delete(Guid activityId);
    }
}
