using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using SOUPIShared.Dtos;
using SOUPIShared.Models; 
using SOUPIShared.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;


namespace SOUPI.Services
{
    public class ProjectService : IProjectService 
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly HttpClient _httpClient;

        private const string createUrl = "/api/project/create";
        private const string getByLoginUrl = "/api/project/getbylogin/";
        private const string updateUrl = "/api/project/update/"; 
        private const string deleteUrl = "/api/project/delete/"; 

        public ProjectService(ILogger<ProjectService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProjectDto>> GetByLogin(string login)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByLoginUrl}{login}");

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

        public async Task<ProjectDto> Update(ProjectDto changedProjectDto)
        {
            try
            {
                var changedProject = new Project(changedProjectDto);

                var context = new ValidationContext(changedProject);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(changedProject, context, results, true))
                {
                    throw new SoupiException(results.First().ErrorMessage);
                }

                var patchDoc = new JsonPatchDocument<Project>();
                patchDoc.Replace(s => s.Name, changedProjectDto.Name)
                    .Replace(s => s.Description, changedProjectDto.Description)
                    .Replace(s => s.Image, changedProjectDto.Image); 

                var content = new StringContent(JsonConvert.SerializeObject(patchDoc), Encoding.UTF8, "application/json-patch+json");
                var response = await _httpClient.PatchAsync($"{updateUrl}/{changedProjectDto.Id}", content);

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
