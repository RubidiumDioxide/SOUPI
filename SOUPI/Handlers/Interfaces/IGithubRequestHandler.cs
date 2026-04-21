using Octokit;
using GithubUser = Octokit.User;


namespace SOUPI.Handlers.Interfaces
{
    public interface IGithubRequestHandler
    {
        public Task<bool> IsAppInstalled(); 

        public Task<GithubUser> GetCurrentUser();

        public Task<GithubUser> GetUserByLogin(string login);

        public Task<IEnumerable<GithubUser>> GetUsersByLogins(IEnumerable<string> logins); 

        public Task<IEnumerable<Repository>> GetRepositoriesForCurrentUser();

        public Task CreateHook(string ownerLogin, string repoName);

        public Task DeleteHook(string ownerLogin, string repoName);
    }
}
