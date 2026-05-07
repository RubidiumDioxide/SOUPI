using SOUPIShared.Dtos.SOUPIDtos;
using static SOUPICore.Misc.GitHubPushPayload; 


namespace SOUPICore.Services.Interfaces
{
    public interface IActivityService
    {
        public Task CreateSet(ILookup<string, CommitInfo> jobsCommits, CancellationToken ct = default);

        public Task<IEnumerable<ActivityDisplayDto>> GetByAssignmentId(Guid assignmentId, CancellationToken ct = default);

        public Task<IEnumerable<ActivityDisplayDto>> GetByTeamMemberId(Guid teamMemberId, CancellationToken ct = default);

        public Task<IEnumerable<ActivityDisplayDto>> GetByJobId(Guid jobId, CancellationToken ct = default);

        public Task<IEnumerable<ActivityDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default); 

        public Task<ActivityDto> Create(ActivityDto newActivityDto, CancellationToken ct = default);

        public Task<ActivityDto> UpdateContent(ActivityDto updatedActivityDto, CancellationToken ct = default);

        public Task Delete(Guid activityId, CancellationToken ct = default); 
    }
}
