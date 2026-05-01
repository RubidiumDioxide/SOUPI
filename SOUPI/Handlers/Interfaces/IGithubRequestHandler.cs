using SOUPIShared.Dtos.OctokitDtos; 


namespace SOUPI.Handlers.Interfaces
{
    public interface IGitHubRequestHandler
    {
        public Task<bool> IsAppInstalled(  ); 

        public Task<GitHubUserDto> GetCurrentUser();

        public Task<GitHubUserDto> GetUserByLogin(string login);

        public Task<IEnumerable<GitHubUserDto>> GetUsersByLogins(IEnumerable<string> logins);

        public Task<GitHubCommitDto> GetCommitByHash(string ownerLogin, string repository, string hash); 

        public Task<IEnumerable<GitHubRepositoryDto>> GetRepositoriesForCurrentUser();

        public Task CreateHook(string ownerLogin, string repoName);

        public Task DeleteHook(string ownerLogin, string repoName);
    }
}
