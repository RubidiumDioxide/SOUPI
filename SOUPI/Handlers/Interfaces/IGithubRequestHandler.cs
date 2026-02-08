using Octokit;
using GithubUser = Octokit.User;


namespace SOUPI.Handlers.Interfaces
{
    public interface IGithubRequestHandler
    {
        public Task<GithubUser> GetCurrentUser();

        public Task<GithubUser> GetUserByLogin(string login);

        public Task<IEnumerable<GithubUser>> GetUsersByLogins(IEnumerable<string> logins); 

        public Task<IEnumerable<Repository>> GetRepositoriesForCurrentUser(); 
    }
}
