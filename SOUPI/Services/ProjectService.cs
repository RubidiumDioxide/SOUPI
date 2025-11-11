using Microsoft.AspNetCore.WebUtilities;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using System.Text.Json;


namespace SOUPI.Services
{
    public class ProjectService : IProjectService 
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly HttpClient _httpClient;

        private const string createUrl = "/api/project/create";
        private const string getUrl = "/api/project/getbylogin/";

        public ProjectService(ILogger<ProjectService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProjectDto>> GetByLogin(string login)
        {
            try
            {
                var queryParams = new Dictionary<string, string>
                {
                    ["login"] = login
                };
                var requestUrl = QueryHelpers.AddQueryString(getUrl, queryParams);

                var response = await _httpClient.GetAsync(requestUrl);

                var newContent = await response.Content.ReadAsStringAsync();

                var projectDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ProjectDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (projectDtos == null)
                {
                    projectDtos = new List<ProjectDto>(); 
                }

                return projectDtos!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить проекты {ex.Message}");
                throw new SoupiException("Не удалось загрузить проекты. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<ProjectDto> Create(ProjectDto projectDto)
        {
            try
            {
                var response = await _httpClient.PostAsync(createUrl, JsonContent.Create(projectDto));

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var newProjectDto = System.Text.Json.JsonSerializer.Deserialize<ProjectDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newProjectDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось создать новый проект. {ex.Message}");
                throw new SoupiException("Не удалось создать новый проект. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        } 
    }
}
