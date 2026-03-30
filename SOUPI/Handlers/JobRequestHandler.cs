using SOUPI.Handlers.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using System.Text.Json;

namespace SOUPI.Handlers
{
    public class JobRequestHandler : IJobRequestHandler
    {
        private readonly ILogger<JobRequestHandler> _logger;
        private readonly HttpClient _httpClient;

        private const string getByIdUrl = "/api/job/getbyid/";
        private const string getByProjectIdUrl = "/api/job/getbyprojectid/";
        private const string createUrl = "/api/job/create/";
        private const string updateContentUrl = "/api/job/updatecontent/";
        private const string updateParentUrl = "/api/job/updateparent/";
        private const string deleteUrl = "/api/job/delete/";

        public JobRequestHandler(ILogger<JobRequestHandler> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<JobDto> GetById(Guid jobId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByIdUrl}{jobId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var jobDto = System.Text.Json.JsonSerializer.Deserialize<JobDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return jobDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить задачу {ex.Message}");
                throw new SoupiException("Не удалось загрузить задачу. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<JobDto>> GetByProjectId(Guid projectId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByProjectIdUrl}{projectId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var jobDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<JobDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return jobDtos!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить задачи {ex.Message}");
                throw new SoupiException("Не удалось загрузить задачи. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<JobDto> Create(JobDto jobDto)
        {
            try
            {
                var response = await _httpClient.PostAsync(createUrl, JsonContent.Create(jobDto));

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var newJobDto = System.Text.Json.JsonSerializer.Deserialize<JobDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newJobDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось создать новую задачу. {ex.Message}");
                throw new SoupiException("Не удалось создать новую задачу. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<JobDto> UpdateContent(JobDto updatedJobDto)
        {
            try
            {
                var content = JsonContent.Create(updatedJobDto);

                var response = await _httpClient.PostAsync(updateContentUrl, content);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var result = System.Text.Json.JsonSerializer.Deserialize<JobDto>(newContent, new JsonSerializerOptions
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

        public async Task<JobDto> UpdateParent(JobDto updatedJobDto)
        {
            try
            {
                var content = JsonContent.Create(updatedJobDto);

                var response = await _httpClient.PostAsync(updateParentUrl, content);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var result = System.Text.Json.JsonSerializer.Deserialize<JobDto>(newContent, new JsonSerializerOptions
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

        public async Task Delete(Guid jobId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{deleteUrl}{jobId}");

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить задачу: {ex.Message}");
                throw new SoupiException("Не удалось удалить задачу ");
            }
        }
    }
}
