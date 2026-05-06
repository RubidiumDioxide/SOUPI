using SOUPIShared.Dtos.OctokitDtos;
using SOUPIShared.Dtos.SOUPIDtos; 


namespace SOUPI.Handlers.Interfaces
{
    public interface IGitHubRequestHandler
    {
        public Task<bool> IsAppInstalled(); 

        public Task<GitHubUserDto> GetCurrentUser();

        public Task<GitHubUserDto> GetUserByLogin(string login);

        public Task<IEnumerable<GitHubUserDto>> GetUsersByLogins(IEnumerable<string> logins);

        public Task<GitHubCommitDto> GetCommitByHash(ProjectDisplayDto project, string hash); 

        public Task<IEnumerable<GitHubRepositoryDto>> GetRepositoriesForCurrentUser();

        public Task<GitHubRepositoryDto> GetRepository(ProjectDisplayDto project);

        public Task<bool> DoesHookExist(ProjectDisplayDto project); 

        public Task CreateHook(ProjectDisplayDto project);

        public Task DeleteHook(ProjectDisplayDto project);
    }
}
