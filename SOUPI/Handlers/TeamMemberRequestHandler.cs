using SOUPI.Handlers.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using System.Text.Json;


namespace SOUPI.Handlers
{
    public class TeamMemberRequestHandler : ITeamMemberRequestHandler 
    {
        private readonly ILogger<TeamMemberRequestHandler> _logger;
        private readonly HttpClient _httpClient;

        private const string getByProjectIdUrl = "/api/TeamMember/getbyprojectid/";  
        private const string createUrl = "/api/TeamMember/create"; 
        private const string updateUrl = "/api/TeamMember/update/";
        private const string deleteUrl = "/api/TeamMember/deleteById/"; 

        public TeamMemberRequestHandler(ILogger<TeamMemberRequestHandler> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<TeamMemberDisplayDto>> GetByProjectId(Guid projectId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByProjectIdUrl}{projectId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var TeamMemberDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<TeamMemberDisplayDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return TeamMemberDtos!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить участников команды проекта: {ex.Message}");
                throw new SoupiException("Не удалось загрузить участников команды проекта. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<TeamMemberDto> Create(TeamMemberDto TeamMemberDto)
        {
            try
            {
                var response = await _httpClient.PostAsync(createUrl, JsonContent.Create(TeamMemberDto));

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var newTeamMemberDto = System.Text.Json.JsonSerializer.Deserialize<TeamMemberDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newTeamMemberDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось добавить в команду проекта нового участника. {ex.Message}");
                throw new SoupiException("Не удалось добавить в команду проекта нового участника. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<TeamMemberDto> Update(TeamMemberDto updatedTeamMemberDto)
        {
            try
            {
                var content = JsonContent.Create(updatedTeamMemberDto);

                var response = await _httpClient.PostAsync(updateUrl, content);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var result = System.Text.Json.JsonSerializer.Deserialize<TeamMemberDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось сохранить изменения: {ex.Message}");
                throw new SoupiException("Не удалось сохранить изменения. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task DeleteById(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{deleteUrl}{id}");

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось исключить участника из команды проекта: {ex.Message}");
                throw new SoupiException("Не удалось исключить участника из команды проекта. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }
    }
}
