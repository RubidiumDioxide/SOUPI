using Octokit;
using GithubUser = Octokit.User;
using SOUPI.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Octokit.Internal;


namespace SOUPI.Services
{
    public class GithubService : IGithubService
    {
        private readonly ILogger<GithubService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GithubService(ILogger<GithubService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GithubUser> GetUser()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                var accessToken = await httpContext!.GetTokenAsync("access_token");

                var github = new GitHubClient(
                    new ProductHeaderValue("AspNetCoreGitBubAuth"),
                    new InMemoryCredentialStore(new Credentials(accessToken))
                );

                return await github.User.Current();
            }
            catch(Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw new SoupiException("Не удалось получить информацию о текущем пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }
    }
}
