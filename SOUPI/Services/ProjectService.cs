using SOUPIShared.Exceptions;
using SOUPIShared.Dtos;
using System.Text.Json;


namespace SOUPI.Services
{
    public class ProjectService : IProjectService 
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly HttpClient _httpClient;

        public ProjectService(ILogger<ProjectService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<ProjectDto> CreateProject(ProjectDto projectDto)
        {
            try
            {
                var response = await _httpClient.PostAsync("/api/project/createproject", JsonContent.Create(projectDto));

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
