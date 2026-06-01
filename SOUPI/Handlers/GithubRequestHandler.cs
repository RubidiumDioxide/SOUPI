using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Octokit; 
using SOUPI.Handlers.Interfaces;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.OctokitDtos;
using SOUPIShared.Dtos.SOUPIDtos;  
using SOUPIShared.Exceptions;


namespace SOUPI.Handlers 
{
    public class GitHubRequestHandler : IGitHubRequestHandler 
    {
        private readonly ILogger<GitHubRequestHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IKeyGenService _keyGenService;
        private readonly string _devtunnelUrl;
        private readonly string _callbackUrl = "api/webhook/push/"; 

        public GitHubRequestHandler(
            ILogger<GitHubRequestHandler> logger, 
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory, 
            IKeyGenService keyGenService, 
            string devtunnelUrl)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor; 
            _httpClientFactory = httpClientFactory; 
            _keyGenService = keyGenService; 
            _devtunnelUrl = devtunnelUrl;
        }

        public async Task<bool> IsAppInstalled(CancellationToken ct = default)
        {
            try
            {
                var github = await GetClientAsync(); 

                var installations = await github.GitHubApps.GetAllInstallationsForCurrentUser();

                return installations.Installations.Count != 0; 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить данные об установленных приложениях {ex.Message}");
                throw new SoupiException("Не удалось получить данные об установленных приложениях. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<GitHubUserDto> GetCurrentUser(CancellationToken ct = default)
        {
            try
            {
                var github = await GetClientAsync();

                var user = await github.User.Current();

                return new GitHubUserDto(user);
            }
            catch(AuthorizationException) { throw; }
            catch(Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о текущем пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о текущем пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<GitHubUserDto> GetUserByLogin(string login, CancellationToken ct = default)
        {
            try
            {
                var github = await GetClientAsync();

                var user = await github.User.Get(login);

                return new GitHubUserDto(user); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о текущем пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о текущем пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        } 

        public async Task<IEnumerable<GitHubUserDto>> GetUsersByLogins(IEnumerable<string> logins, CancellationToken ct = default)
        {
            try
            {
                var github = await GetClientAsync();

                var tasks = logins.Select(l => github.User.Get(l)); 
                var users = await Task.WhenAll(tasks);

                return users.Select(u => new GitHubUserDto(u)).ToList(); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о текущем пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о текущем пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<GitHubCommitDto> GetCommitByHash(ProjectDisplayDto project, string hash, CancellationToken ct = default)
        {
            try
            {
                var github = await GetClientAsync();

                var commit = await github.Repository.Commit.Get(project.CreatorLogin, project.GithubRepository, hash);

                return new GitHubCommitDto(commit);   
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о коммите.  {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о коммите. Проверьте правильность хэша. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<GitHubRepositoryDto>> GetRepositoriesForCurrentUser(CancellationToken ct = default)
        {
            try
            {
                var github = await GetClientAsync(); 

                var repositories = await github.Repository.GetAllForCurrent();

                return repositories.Select(r => new GitHubRepositoryDto(r)).ToList(); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о репозиториях текущего пользователя. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о репозиториях текущего пользователя. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }
        
        public async Task<GitHubRepositoryDto> GetRepository(ProjectDisplayDto project, CancellationToken ct = default)
        {
            try
            {
                var github = await GetClientAsync(); 

                var repo = await github.Repository.Get(project.CreatorLogin, project.GithubRepository);

                return new GitHubRepositoryDto(repo);    
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о репозитории. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о репозитории. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<bool> DoesHookExist(ProjectDisplayDto project, CancellationToken ct = default)
        {
            try
            {
                if (_devtunnelUrl == null)
                {
                    throw new Exception("Не было предоставлено значение для devtunnelUrl. Убедитесь, что приложение запущено с активным туннелем ");
                }

                var github = await GetClientAsync();
                
                var fullCallbackUrl = $"{_devtunnelUrl}{_callbackUrl}";

                var repository = await github.Repository.Get(project.CreatorLogin, project.GithubRepository);

                // fetch all hooks and find all matches for the URL
                var allHooks = await github.Repository.Hooks.GetAll(project.CreatorLogin, project.GithubRepository);

                var duplicateHooks = allHooks.Where(h =>
                    h.Config.TryGetValue("url", out var url) && url == fullCallbackUrl).ToList();

                // delete every matching hook found
                return duplicateHooks.Count != 0;  
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о наличии вебхука {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о наличии вебхука. Попробуйте позже или сообщите об ошибке в техподдержку");
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
        public async Task CreateHook(ProjectDisplayDto project, CancellationToken ct = default) 
        {
            try
            {
                if(_devtunnelUrl == null)
                {
                    throw new Exception("Не было предоставлено значение для devtunnelUrl. Убедитесь, что приложение запущено с активным туннелем "); 
                }

                var github = await GetClientAsync(); 

                var fullCallbackUrl = $"{_devtunnelUrl}{_callbackUrl}"; 

                var repository = await github.Repository.Get(project.CreatorLogin, project.GithubRepository);

                // fetch all hooks and find all matches for the URL
                var allHooks = await github.Repository.Hooks.GetAll(project.CreatorLogin, project.GithubRepository);

                var duplicateHooks = allHooks.Where(h =>
                    h.Config.TryGetValue("url", out var url) && url == fullCallbackUrl).ToList();

                // delete every matching hook found
                if (duplicateHooks.Any())
                {
                    foreach (var hook in duplicateHooks)
                    {
                        await github.Repository.Hooks.Delete(project.CreatorLogin, project.GithubRepository, hook.Id);
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

                await github.Repository.Hooks.Create(project.CreatorLogin, project.GithubRepository, newHook);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось создать вебхук. {ex.Message}");
                throw new SoupiException("Не удалось создать вебхук. Попробуйте позже или сообщите об ошибке в техподдержку");
            }
        }

        public async Task DeleteHook(ProjectDisplayDto project, CancellationToken ct = default)
        {
            try
            {
                if (_devtunnelUrl == null)
                {
                    throw new Exception("Не было предоставлено значение для devtunnelUrl. Убедитесь, что приложение запущено с активным туннелем ");
                }

                var github = await GetClientAsync(); 

                var fullCallbackUrl = $"{_devtunnelUrl}{_callbackUrl}"; 

                var repository = await github.Repository.Get(project.CreatorLogin, project.GithubRepository);

                // fetch all hooks and find all matches for the URL
                var allHooks = await github.Repository.Hooks.GetAll(project.CreatorLogin, project.GithubRepository);

                var matchingHooks = allHooks.Where(h =>
                    h.Config.TryGetValue("url", out var url) && url == fullCallbackUrl).ToList();

                // delete every matching hook found
                if (matchingHooks.Any())
                {
                    foreach (var hook in matchingHooks)
                    {
                        await github.Repository.Hooks.Delete(project.CreatorLogin, project.GithubRepository, hook.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить вебхук. {ex.Message}");
                throw new SoupiException("Не удалось удалить вебхук. Попробуйте позже или сообщите об ошибке в техподдержку");
            }
        }

        private async Task<GitHubClient> GetClientAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("No active HTTP context available.");
            }

            var accessToken = await httpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                throw new AuthorizationException();
            }

            var client = new GitHubClient(new ProductHeaderValue("AspNetCoreGitHubAuth"))
            {
                Credentials = new Credentials(accessToken)
            };

            try
            {
                // Verify the token works
                await client.User.Current();
            }
            catch (AuthorizationException)
            {
                _logger.LogWarning("GitHub token is invalid or expired.");
                throw;
            }

            return client;
        }
    }
}
