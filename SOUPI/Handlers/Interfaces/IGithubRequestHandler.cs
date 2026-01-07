using Octokit;
using GithubUser = Octokit.User;


namespace SOUPI.Handlers.Interfaces
{
    public interface IGithubRequestHandler
    {
        public Task<GithubUser> GetUser();
        public Task<IEnumerable<Repository>> GetRepositoriesForCurrentUser(); 
    }
}
