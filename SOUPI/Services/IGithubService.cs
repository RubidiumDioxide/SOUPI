using GithubUser = Octokit.User; 


namespace SOUPI.Services
{
    public interface IGithubService
    {
        public Task<GithubUser> GetUser(); 
    }
}
