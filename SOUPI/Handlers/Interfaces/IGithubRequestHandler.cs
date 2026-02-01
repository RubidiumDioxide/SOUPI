using Octokit;
using GithubUser = Octokit.User;


namespace SOUPI.Handlers.Interfaces
{
    public interface IGithubRequestHandler
    {
        public Task<GithubUser> GetCurrentUser();

        public Task<GithubUser> GetUserByLogin(string login);

        public Task<IEnumerable<Repository>> GetRepositoriesForCurrentUser(); 
    }
}
