using SOUPIShared.Dtos;
using static SOUPIShared.Dtos.GitHubPushPayload;


namespace SOUPICore.Services.Interfaces
{
    public interface IActivityService
    {
        public Task<IEnumerable<ActivityDto>> CreateSet(ILookup<string, CommitInfo> jobsCommits); 
    }
}
