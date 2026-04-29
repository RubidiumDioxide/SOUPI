using Octokit;
using GitHubUser = Octokit.User;


namespace SOUPI.Handlers.Interfaces
{
    public interface IGitHubRequestHandler
    {
        public Task<bool> IsAppInstalled(); 

        public Task<GitHubUser> GetCurrentUser();

        public Task<GitHubUser> GetUserByLogin(string login);

        public Task<IEnumerable<GitHubUser>> GetUsersByLogins(IEnumerable<string> logins);

        public Task<GitHubCommit> GetCommitByHash(string ownerLogin, string repository, string hash); 

        public Task<IEnumerable<Repository>> GetRepositoriesForCurrentUser();

        public Task CreateHook(string ownerLogin, string repoName);

        public Task DeleteHook(string ownerLogin, string repoName);
    }
}
