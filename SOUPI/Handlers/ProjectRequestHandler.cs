using SOUPI.Handlers.Interfaces;
using SOUPIShared.Exceptions;
using System.Text.Json;
using System.Net;
using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers
{
    public class ProjectRequestHandler : IProjectRequestHandler 
    {
        private readonly ILogger<ProjectRequestHandler> _logger;
        private readonly HttpClient _httpClient;

        private const string createUrl = "/api/project/create/";
        private const string getByUserIdUrl = "/api/project/getbyuserid/";
        private const string getByIdUrl = "/api/project/getbyid/";
        private const string updateUrl = "/api/project/update/"; 
        private const string updateCreatorUrl = "/api/project/updateCreator/"; 
        private const string setGitHubRepositoryUrl = "/api/project/setGitHubRepository/"; 
        private const string deleteUrl = "/api/project/delete/"; 

        public ProjectRequestHandler(ILogger<ProjectRequestHandler> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProjectDisplayDto>> GetByUserId(Guid userId, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByUserIdUrl}{userId}", ct);

                response.EnsureSuccessStatusCode(); 

                var newContent = await response.Content.ReadAsStringAsync(ct);

                var projectDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ProjectDisplayDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return projectDtos!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить проекты {ex.Message}");
                throw new SoupiException("Не удалось загрузить проекты. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<ProjectDisplayDto?> GetById(Guid id, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByIdUrl}{id}", ct);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                }

                response.EnsureSuccessStatusCode(); 

                var newContent = await response.Content.ReadAsStringAsync(ct);

                var projectDto = System.Text.Json.JsonSerializer.Deserialize<ProjectDisplayDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return projectDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить проект {ex.Message}");
                throw new SoupiException("Не удалось загрузить проект. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<ProjectDto> Create(ProjectDto projectDto, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.PostAsync(createUrl, JsonContent.Create(projectDto), ct);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

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

        public async Task<ProjectDto> Update(ProjectDto updatedProjectDto, CancellationToken ct = default)
        {
            try
            {
                var content = JsonContent.Create(updatedProjectDto);

                var response = await _httpClient.PostAsync(updateUrl, content, ct);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

                var result = System.Text.Json.JsonSerializer.Deserialize<ProjectDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось сохранить изменения: {ex.Message}");
                throw new SoupiException("Не удалось сохранить изменения ");
            }
        }

        public async Task<ProjectDto> UpdateCreator(ProjectDto updatedProjectDto, CancellationToken ct = default)
        {
            try
            {
                var content = JsonContent.Create(updatedProjectDto);

                var response = await _httpClient.PostAsync(updateCreatorUrl, content, ct);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

                var result = System.Text.Json.JsonSerializer.Deserialize<ProjectDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось передать управление проектом: {ex.Message}");
                throw new SoupiException("Не удалось передать управление проектом ");
            }
        }

        public async Task<ProjectDto> SetGitHubRepository(Guid projectId, string repositoryName, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{setGitHubRepositoryUrl}{projectId}/{repositoryName}", ct);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

                var result = System.Text.Json.JsonSerializer.Deserialize<ProjectDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось привязать репозиторий к проекту: {ex.Message}");
                throw new SoupiException("Не удалось привязать репозиторий к проекту ");
            }
        }

        public async Task Delete(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{deleteUrl}{projectId}", ct);

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить проект: {ex.Message}");
                throw new SoupiException("Не удалось удалить проект ");
            }
        }
    }
}
