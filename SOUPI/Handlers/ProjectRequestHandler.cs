using SOUPI.Handlers.Interfaces; 
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using System.Text.Json;
using System.Net;


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
        private const string changeCreatorUrl = "/api/project/changeCreator/"; 
        private const string deleteUrl = "/api/project/delete/"; 

        public ProjectRequestHandler(ILogger<ProjectRequestHandler> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProjectDto>> GetByUserId(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByUserIdUrl}{userId}");

                response.EnsureSuccessStatusCode(); 

                var newContent = await response.Content.ReadAsStringAsync();

                var projectDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ProjectDto>>(newContent, new JsonSerializerOptions
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

        public async Task<ProjectDto> GetById(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByIdUrl}{id}");

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                }

                response.EnsureSuccessStatusCode(); 

                var newContent = await response.Content.ReadAsStringAsync();

                var projectDto = System.Text.Json.JsonSerializer.Deserialize<ProjectDto>(newContent, new JsonSerializerOptions
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

        public async Task<ProjectDto> Update(ProjectDto updatedProjectDto)
        {
            try
            {
                var content = JsonContent.Create(updatedProjectDto);

                var response = await _httpClient.PostAsync(updateUrl, content);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

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

        public async Task<ProjectDto> ChangeCreator(ProjectDto updatedProjectDto)
        {
            try
            {
                var content = JsonContent.Create(updatedProjectDto);

                var response = await _httpClient.PostAsync(changeCreatorUrl, content);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

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

        public async Task Delete(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{deleteUrl}{id}");

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
