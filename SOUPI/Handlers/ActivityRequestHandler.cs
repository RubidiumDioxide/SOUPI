using SOUPI.Handlers.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using System.Text.Json;


namespace SOUPI.Handlers
{
    public class ActivityRequestHandler : IActivityRequestHandler
    {
        private readonly ILogger<ActivityRequestHandler> _logger;
        private readonly HttpClient _httpClient;

        private const string getByAssignmentIdUrl = "/api/activity/getbyassignmentid/";
        private const string getByTeamMemberIdUrl = "/api/activity/getbyteammemberid/";
        private const string getByJobIdUrl = "/api/activity/getbyjobid/"; 
        private const string getByProjectIdUrl = "/api/activity/getbyprojectid/"; 
        private const string createUrl = "/api/activity/create/";
        private const string updateContentUrl = "/api/activity/updatecontent/";
        private const string deleteUrl = "/api/activity/delete/";

        public ActivityRequestHandler(ILogger<ActivityRequestHandler> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByAssignmentId(Guid assignmentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByAssignmentIdUrl}{assignmentId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var activityDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ActivityDisplayDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return activityDtos!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить действия {ex.Message}");
                throw new SoupiException("Не удалось загрузить действия. Попробуйте позже или сообщите об ошибке в техподдержку "); 
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByTeamMemberId(Guid teamMemberId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByTeamMemberIdUrl}{teamMemberId}"); 

                response.EnsureSuccessStatusCode(); 

                var newContent = await response.Content.ReadAsStringAsync();

                var activityDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ActivityDisplayDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return activityDtos!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить действия {ex.Message}");
                throw new SoupiException("Не удалось загрузить действия. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByJobId(Guid jobId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByJobIdUrl}{jobId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var activityDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ActivityDisplayDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return activityDtos!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить действия {ex.Message}");
                throw new SoupiException("Не удалось загрузить действия. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByProjectId(Guid projectId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByProjectIdUrl}{projectId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var activityDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ActivityDisplayDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return activityDtos!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить действия {ex.Message}");
                throw new SoupiException("Не удалось загрузить действия. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<ActivityDto> Create(ActivityDto activityDto)
        {
            try
            {
                var response = await _httpClient.PostAsync(createUrl, JsonContent.Create(activityDto)); 

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var newActivityDto = System.Text.Json.JsonSerializer.Deserialize<ActivityDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newActivityDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось добавить действие {ex.Message}");
                throw new SoupiException("Не удалось добавить действие. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<ActivityDto> UpdateContent(ActivityDto updatedActivityDto)
        {
            try
            {
                var response = await _httpClient.PostAsync(updateContentUrl, JsonContent.Create(updatedActivityDto));

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var newUpdatedActivityDto = System.Text.Json.JsonSerializer.Deserialize<ActivityDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newUpdatedActivityDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось изменить действие {ex.Message}");
                throw new SoupiException("Не удалось изменить действие. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task Delete(Guid activityId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{deleteUrl}{activityId}");

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить действие {ex.Message}");
                throw new SoupiException("Не удалось удалить действие ");
            }
        }
    }
}
