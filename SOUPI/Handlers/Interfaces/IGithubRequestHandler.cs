using SOUPIShared.Dtos.OctokitDtos; 


namespace SOUPI.Handlers.Interfaces
{
    public interface IGitHubRequestHandler
    {
        public Task<bool> IsAppInstalled(); 

        public Task<GitHubUserDto> GetCurrentUser();

        public Task<GitHubUserDto> GetUserByLogin(string login);

        public Task<IEnumerable<GitHubUserDto>> GetUsersByLogins(IEnumerable<string> logins);

        public Task<GitHubCommitDto> GetCommitByHash(string owner, string repository, string hash); 

        public Task<IEnumerable<GitHubRepositoryDto>> GetRepositoriesForCurrentUser();

        public Task<GitHubRepositoryDto> GetRepository(string owner, string repository);

        public Task<bool> DoesHookExist(string owner, string repository); 

        public Task CreateHook(string owner, string repository);

        public Task DeleteHook(string owner, string repository);
    }
}
