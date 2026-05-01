using SOUPI.Handlers.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;
using System.Text.Json;


namespace SOUPI.Handlers
{
    public class JobRequestHandler : IJobRequestHandler
    {
        private readonly ILogger<JobRequestHandler> _logger;
        private readonly HttpClient _httpClient;

        private const string getDisplayByIdUrl = "/api/job/getdisplaybyid/";
        private const string getDisplayByProjectIdUrl = "/api/job/getdisplaybyprojectid/";
        private const string getDisplayByProjectIdParentIdUrl = "/api/job/getdisplaybyprojectidparentid/";
        private const string getByIdUrl = "/api/job/getbyid/";
        private const string getByProjectIdUrl = "/api/job/getbyprojectid/";
        private const string getByProjectIdParentIdUrl = "/api/job/getbyprojectidparentid/";
        private const string createUrl = "/api/job/create/";
        private const string updateContentUrl = "/api/job/updatecontent/";
        private const string updateParentUrl = "/api/job/updateparent/";
        private const string deleteUrl = "/api/job/delete/";

        public JobRequestHandler(ILogger<JobRequestHandler> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<JobDisplayDto> GetDisplayById(Guid jobId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getDisplayByIdUrl}{jobId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var jobDto = System.Text.Json.JsonSerializer.Deserialize<JobDisplayDto>(newContent, new JsonSerializerOptions
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

        public async Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectId(Guid projectId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getDisplayByProjectIdUrl}{projectId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var jobDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<JobDisplayDto>>(newContent, new JsonSerializerOptions
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

        public async Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectIdParentId(Guid projectId, Guid? parentJobId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getDisplayByProjectIdParentIdUrl}{projectId}/{parentJobId}"); 

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var jobDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<JobDisplayDto>>(newContent, new JsonSerializerOptions
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

        public async Task<IEnumerable<JobDto>> GetByProjectIdParentId(Guid projectId, Guid? parentJobId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByProjectIdParentIdUrl}{projectId}/{parentJobId}");

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

        public async Task<JobDto> UpdateParent(Guid jobId, Guid? newParentJobId)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    ((newParentJobId == null) ? $"{updateParentUrl}{jobId}" : $"{updateParentUrl}{jobId}/{newParentJobId}"));

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

        public async Task Delete(Guid jobId, bool preserveChildren)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{deleteUrl}{jobId}/{preserveChildren}");

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
