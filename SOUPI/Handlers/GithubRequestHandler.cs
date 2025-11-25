using Octokit;
using GithubUser = Octokit.User;
using SOUPIShared.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Octokit.Internal;


namespace SOUPICore.Services
{
    public class GithubRequestHandler 
    {
        private readonly ILogger<GithubRequestHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GithubRequestHandler(ILogger<GithubRequestHandler> logger, IHttpContextAccessor httpContextAccessor)
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
                _logger.LogError($"Не удалось получить информацию о текущем пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о текущем пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<Repository>> GetRepositoriesForCurrent()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                var accessToken = await httpContext!.GetTokenAsync("access_token");

                var github = new GitHubClient(
                    new ProductHeaderValue("AspNetCoreGitBubAuth"),
                    new InMemoryCredentialStore(new Credentials(accessToken))
                ); 

                return await github.Repository.GetAllForCurrent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о репозиториях текущего пользователя. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о репозиториях текущего пользователя. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }
    }
}
