using SOUPIShared.Dtos;
using static SOUPIShared.Dtos.GitHubPushPayload;


namespace SOUPICore.Services.Interfaces
{
    public interface IActivityService
    {
        public Task<IEnumerable<ActivityDto>> CreateSet(ILookup<string, CommitInfo> jobsCommits);

        public Task<IEnumerable<ActivityDisplayDto>> GetByAssignmentId(Guid assignmentId);

        public Task<IEnumerable<ActivityDisplayDto>> GetByTeamMemberId(Guid teamMemberId);

        public Task<IEnumerable<ActivityDisplayDto>> GetByJobId(Guid jobId);

        public Task<IEnumerable<ActivityDisplayDto>> GetByProjectId(Guid projectId); 

        public Task<ActivityDto> Create(ActivityDto newActivityDto);

        public Task<ActivityDto> UpdateContent(ActivityDto updatedActivityDto);

        public Task Delete(Guid activityId); 
    }
}
