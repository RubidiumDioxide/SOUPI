using GithubUser = Octokit.User;
using Octokit; 

namespace SOUPI.Services
{
    public interface IGithubService
    {
        public Task<GithubUser> GetUser();

        public Task<IEnumerable<Repository>> GetRepositories(); 
    }
}
