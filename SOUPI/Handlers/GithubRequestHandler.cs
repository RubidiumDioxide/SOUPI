using Microsoft.AspNetCore.Authentication;
using Octokit;
using Octokit.Internal;
using SOUPI.Handlers.Interfaces;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Exceptions;
using GitHubUser = Octokit.User;


namespace SOUPI.Handlers 
{
    public class GitHubRequestHandler : IGitHubRequestHandler 
    {
        private readonly ILogger<GitHubRequestHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IKeyGenService _keyGenService;
        private readonly string _devtunnelUrl;
        private readonly string _callbackUrl = "api/webhook/push/"; 

        public GitHubRequestHandler(ILogger<GitHubRequestHandler> logger, IHttpContextAccessor httpContextAccessor, IKeyGenService keyGenService, string devtunnelUrl)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor; 
            _keyGenService = keyGenService; 
            _devtunnelUrl = devtunnelUrl;
        }

        public async Task<bool> IsAppInstalled()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                var accessToken = await httpContext!.GetTokenAsync("access_token");

                var github = new GitHubClient(
                    new ProductHeaderValue("AspNetCoreGitHubAuth"),
                    new InMemoryCredentialStore(new Credentials(accessToken))
                );

                var installations = await github.GitHubApps.GetAllInstallationsForCurrentUser(); 

                return installations.Installations.Any(); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось перейти на страницу установки GithubApp. {ex.Message}");
                throw new SoupiException("Не удалось перейти на страницу установки GithubApp. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<GitHubUser> GetCurrentUser()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                var accessToken = await httpContext!.GetTokenAsync("access_token");

                var github = new GitHubClient(
                    new ProductHeaderValue("AspNetCoreGitHubAuth"),
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

        public async Task<GitHubUser> GetUserByLogin(string login)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                var accessToken = await httpContext!.GetTokenAsync("access_token");

                var github = new GitHubClient(
                    new ProductHeaderValue("AspNetCoreGitHubAuth"),
                    new InMemoryCredentialStore(new Credentials(accessToken))
                );

                return await github.User.Get(login); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о текущем пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о текущем пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<GitHubUser>> GetUsersByLogins(IEnumerable<string> logins)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                var accessToken = await httpContext!.GetTokenAsync("access_token");

                var github = new GitHubClient(
                    new ProductHeaderValue("AspNetCoreGitHubAuth"),
                    new InMemoryCredentialStore(new Credentials(accessToken))
                );

                var tasks = logins.Select(l => github.User.Get(l)); 
                var users = await Task.WhenAll(tasks);

                return users.ToList(); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о текущем пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о текущем пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<Repository>> GetRepositoriesForCurrentUser()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                var accessToken = await httpContext!.GetTokenAsync("access_token");

                var github = new GitHubClient(
                    new ProductHeaderValue("AspNetCoreGitHubAuth"),
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

        /// <summary>
        /// Удаляет все существующие вебхуки с таким же callbackUrl  
        /// </summary>
        /// <param name="ownerLogin">
        ///     Логин владельца репозитория 
        /// </param>
        /// <param name="repoName">
        ///     Название репозитория 
        /// </param>
        /// <param name="callbackUrl">
        ///     Куда будет стучаться вебхук 
        /// </param>
        /// <returns></returns>
        /// <exception cref="SoupiException"></exception>
        public async Task CreateHook(string ownerLogin, string repoName) 
        {
            try
            {
                if(_devtunnelUrl == null)
                {
                    throw new Exception("Не было предоставлено значение для devtunnelUrl. Убедитесь, что приложение запущено с активным туннелем "); 
                }

                var httpContext = _httpContextAccessor.HttpContext;
                var accessToken = await httpContext!.GetTokenAsync("access_token");

                var fullCallbackUrl = $"{_devtunnelUrl}{_callbackUrl}"; 

                var github = new GitHubClient(
                    new ProductHeaderValue("AspNetCoreGitHubAuth"),
                    new InMemoryCredentialStore(new Credentials(accessToken))
                );

                var repository = await github.Repository.Get(ownerLogin, repoName);

                // fetch all hooks and find all matches for the URL
                var allHooks = await github.Repository.Hooks.GetAll(ownerLogin, repoName);

                var duplicateHooks = allHooks.Where(h =>
                    h.Config.TryGetValue("url", out var url) && url == fullCallbackUrl).ToList();

                // delete every matching hook found
                if (duplicateHooks.Any())
                {
                    foreach (var hook in duplicateHooks)
                    {
                        await github.Repository.Hooks.Delete(ownerLogin, repoName, hook.Id);
                    }
                }

                var secret = _keyGenService.GenerateWebhookSecret(repository.Id); 

                // Define the webhook configuration
                var config = new Dictionary<string, string>
                {
                    { "url", fullCallbackUrl }, // Your endpoint that receives the JSON
                    { "content_type", "json" },
                    { "secret", secret } // Recommended for security validation
                };

                var newHook = new NewRepositoryHook("web", config)
                {
                    // Specify which events you want to listen to
                    Events = ["push"],
                    Active = true
                };

                await github.Repository.Hooks.Create(ownerLogin, repoName, newHook);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось создать вебхук. {ex.Message}");
                throw new SoupiException("Не удалось создать вебхук. Попробуйте позже или сообщите об ошибке в техподдержку");
            }
        }

        public async Task DeleteHook(string ownerLogin, string repoName)
        {
            try
            {
                if (_devtunnelUrl == null)
                {
                    throw new Exception("Не было предоставлено значение для devtunnelUrl. Убедитесь, что приложение запущено с активным туннелем ");
                }

                var httpContext = _httpContextAccessor.HttpContext;
                var accessToken = await httpContext!.GetTokenAsync("access_token");

                var fullCallbackUrl = $"{_devtunnelUrl}{_callbackUrl}";

                var github = new GitHubClient(
                    new ProductHeaderValue("AspNetCoreGitHubAuth"),
                    new InMemoryCredentialStore(new Credentials(accessToken))
                );

                var repository = await github.Repository.Get(ownerLogin, repoName);

                // fetch all hooks and find all matches for the URL
                var allHooks = await github.Repository.Hooks.GetAll(ownerLogin, repoName);

                var matchingHooks = allHooks.Where(h =>
                    h.Config.TryGetValue("url", out var url) && url == fullCallbackUrl).ToList();

                // delete every matching hook found
                if (matchingHooks.Any())
                {
                    foreach (var hook in matchingHooks)
                    {
                        await github.Repository.Hooks.Delete(ownerLogin, repoName, hook.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить вебхук. {ex.Message}");
                throw new SoupiException("Не удалось удалить вебхук. Попробуйте позже или сообщите об ошибке в техподдержку");
            }
        }
    }
}
