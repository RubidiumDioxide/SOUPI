using SOUPIShared.Dtos.SOUPIDtos;
using static SOUPICore.Misc.GitHubPushPayload; 


namespace SOUPICore.Services.Interfaces
{
    public interface IActivityService
    {
        public Task CreateSet(ILookup<string, CommitInfo> jobsCommits);

        public Task<IEnumerable<ActivityDisplayDto>> GetByAssignmentId(Guid assignmentId);

        public Task<IEnumerable<ActivityDisplayDto>> GetByTeamMemberId(Guid teamMemberId);

        public Task<IEnumerable<ActivityDisplayDto>> GetByJobId(Guid jobId);

        public Task<IEnumerable<ActivityDisplayDto>> GetByProjectId(Guid projectId); 

        public Task<ActivityDto> Create(ActivityDto newActivityDto);

        public Task<ActivityDto> UpdateContent(ActivityDto updatedActivityDto);

        public Task Delete(Guid activityId); 
    }
}
