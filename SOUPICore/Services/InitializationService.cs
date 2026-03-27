using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SOUPIShared.Models; 


namespace SOUPICore.Services
{
    public class InitializationService : BackgroundService
    {
        private readonly ILogger<InitializationService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public InitializationService(
    ILogger<InitializationService> logger,
    IServiceProvider serviceProvider,
    IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken ct = default)
        {
            var shouldInitializeTestAccounts = _configuration.GetValue<bool>("ShouldInitializeTestAccounts");

            if (shouldInitializeTestAccounts)
            {
                await InitializeTestAccounts(ct); 
            }
        }

        private async Task InitializeTestAccounts(CancellationToken ct = default)
        {
            var testAuthLogin = _configuration.GetValue<string>("TestAuthLogin");

            if (string.IsNullOrWhiteSpace(testAuthLogin))
            {
                _logger.LogError("В конфигурации отсутствует TestAuthLogin");
                return;
            }
            
            var testUserLogins = _configuration.GetSection("TestUserLogins").Get<List<string>>();

            if (testUserLogins == null || testUserLogins.Count == 0)
            {
                _logger.LogError("В конфигурации отсутствует раздел TestUserLogins лиоб он пуст ");
                return;
            }

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SoupiDbContext>();

                var existingUserLogins = await context.Users.Select(u => u.Login).ToListAsync(ct);

                foreach (var login in testUserLogins)
                {
                    if (!existingUserLogins.Contains(login))
                    {
                        var testUser = new User() { Login = login };

                        await context.Users.AddAsync(testUser, ct);
                    }
                }

                await context.SaveChangesAsync(ct);

                _logger.LogInformation("Тестовые пользователи успешно инициализированы из конфигурации");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при инициализации тестовых пользователей"); 
            }
        }
    }
}
