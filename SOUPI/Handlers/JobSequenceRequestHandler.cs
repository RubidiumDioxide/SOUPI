using SOUPI.Handlers.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using System.Text.Json;

namespace SOUPI.Handlers
{
    public class JobSequenceRequestHandler : IJobSequenceRequestHandler
    {
        private readonly ILogger<JobSequenceRequestHandler> _logger;
        private readonly HttpClient _httpClient;

        private const string getByProjectIdUrl = "/api/jobsequence/getbyprojectid/";
        private const string createUrl = "/api/jobsequence/create/";
        private const string deleteUrl = "/api/jobsequence/delete/";

        public JobSequenceRequestHandler(ILogger<JobSequenceRequestHandler> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<JobSequenceDisplayDto>> GetByProjectId(Guid projectId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByProjectIdUrl}{projectId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var jobSequenceDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<JobSequenceDisplayDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return jobSequenceDtos!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить связи между задачами {ex.Message}");
                throw new SoupiException("Не удалось загрузить связи между задачами. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }
        
        public async Task<JobSequenceDto> Create(Guid firstJobId, Guid secondJobId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{createUrl}{firstJobId}/{secondJobId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var newJobSequenceDto = System.Text.Json.JsonSerializer.Deserialize<JobSequenceDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newJobSequenceDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось создать новую связь между задачами. {ex.Message}");
                throw new SoupiException("Не удалось создать новую связь между задачами. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task Delete(Guid jobSequenceId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{deleteUrl}{jobSequenceId}");

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить связь между задачами: {ex.Message}");
                throw new SoupiException("Не удалось удалить связь между задачами ");
            }
        }
    }
}
