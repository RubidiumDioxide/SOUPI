using SOUPIShared.Dtos.OctokitDtos;
using SOUPIShared.Dtos.SOUPIDtos; 


namespace SOUPI.Handlers.Interfaces
{
    public interface IGitHubRequestHandler
    {
        public Task<bool> IsAppInstalled(CancellationToken ct = default); 

        public Task<GitHubUserDto> GetCurrentUser(CancellationToken ct = default);

        public Task<GitHubUserDto> GetUserByLogin(string login, CancellationToken ct = default);

        public Task<IEnumerable<GitHubUserDto>> GetUsersByLogins(IEnumerable<string> logins, CancellationToken ct = default);

        public Task<GitHubCommitDto> GetCommitByHash(ProjectDisplayDto project, string hash, CancellationToken ct = default); 

        public Task<IEnumerable<GitHubRepositoryDto>> GetRepositoriesForCurrentUser(CancellationToken ct = default);

        public Task<GitHubRepositoryDto> GetRepository(ProjectDisplayDto project, CancellationToken ct = default);

        public Task<bool> DoesHookExist(ProjectDisplayDto project, CancellationToken ct = default); 

        public Task CreateHook(ProjectDisplayDto project, CancellationToken ct = default);

        public Task DeleteHook(ProjectDisplayDto project, CancellationToken ct = default);
    }
}
