using SOUPI.Handlers.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;
using System.Text.Json;


namespace SOUPI.Handlers
{
    public class AssignmentRequestHandler : IAssignmentRequestHandler
    {
        private readonly ILogger<AssignmentRequestHandler> _logger;
        private readonly HttpClient _httpClient;

        private const string getByIdUrl = "/api/assignment/getbyid/";
        private const string getByProjectIdUrl = "/api/assignment/getbyprojectid/";
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

        public async Task<AssignmentDisplayDto> GetById(Guid assignmentId, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByIdUrl}{assignmentId}", ct);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

                var assignmentDto = System.Text.Json.JsonSerializer.Deserialize<AssignmentDisplayDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return assignmentDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить исполнителя{ex.Message}");
                throw new SoupiException("Не удалось загрузить исполнителя. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }
        
        public async Task<IEnumerable<AssignmentDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByProjectIdUrl}{projectId}", ct);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

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

        public async Task<IEnumerable<AssignmentDisplayDto>> GetByJobId(Guid jobId, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByJobIdUrl}{jobId}", ct);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

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

        public async Task<IEnumerable<AssignmentDisplayDto>> GetByUserId(Guid userId, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByUserIdUrl}{userId}", ct);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

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

        public async Task<AssignmentDto> Create(AssignmentDto assignmentDto, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.PostAsync(createUrl, JsonContent.Create(assignmentDto), ct);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

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

        public async Task<AssignmentDto> UpdateContent(AssignmentDto updatedAssignmentDto, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.PostAsync(updateContentUrl, JsonContent.Create(updatedAssignmentDto), ct);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

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

        public async Task Delete(Guid assignmentId, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{deleteUrl}{assignmentId}", ct);

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
