using SOUPI.Handlers.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using System.Text.Json;


namespace SOUPI.Handlers
{
    public class AssignmentRequestHandler : IAssignmentRequestHandler
    {
        private readonly ILogger<AssignmentRequestHandler> _logger;
        private readonly HttpClient _httpClient;

        private const string getByJobIdUrl = "/api/assignment/getbyjobid/";
        private const string getByUserIdUrl = "/api/assignment/getbyuserid/";
        private const string createUrl = "/api/assignment/create/";
        private const string updateContentUrl = "/api/assignment/updateContent/";
        private const string deleteUrl = "/api/assignment/delete/";

        public AssignmentRequestHandler(ILogger<AssignmentRequestHandler> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<AssignmentDisplayDto>> GetByJobId(Guid jobId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByJobIdUrl}{jobId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var assignmentDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<AssignmentDisplayDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return assignmentDtos!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить исполнителей {ex.Message}");
                throw new SoupiException("Не удалось загрузить исполнителей. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<AssignmentDisplayDto>> GetByUserId(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByUserIdUrl}{userId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var assignmentDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<AssignmentDisplayDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return assignmentDtos!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить исполнителей {ex.Message}");
                throw new SoupiException("Не удалось загрузить исполнителей. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<AssignmentDto> Create(AssignmentDto assignmentDto)
        {
            try
            {
                var response = await _httpClient.PostAsync(createUrl, JsonContent.Create(assignmentDto));

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var newAssignmentDto = System.Text.Json.JsonSerializer.Deserialize<AssignmentDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newAssignmentDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось добавить исполнителя {ex.Message}");
                throw new SoupiException("Не удалось добавить исполнителя. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<AssignmentDto> UpdateContent(AssignmentDto updatedAssignmentDto)
        {
            try
            {
                var response = await _httpClient.PostAsync(updateContentUrl, JsonContent.Create(updatedAssignmentDto));

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var newUpdatedAssignmentDto = System.Text.Json.JsonSerializer.Deserialize<AssignmentDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newUpdatedAssignmentDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось изменить назначение {ex.Message}");
                throw new SoupiException("Не удалось изменить назначение. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task Delete(Guid assignmentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{deleteUrl}{assignmentId}");

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить исполнителя {ex.Message}");
                throw new SoupiException("Не удалось удалить исполнителя ");
            }
        }
    }
}
