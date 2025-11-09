using SOUPIShared.Exceptions;
using SOUPIShared.Dtos;
using System.Text.Json;


namespace SOUPI.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly HttpClient _httpClient;

        public UserService(ILogger<UserService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient; 
        }

        public async Task<UserDto> SaveNewUser(UserDto userDto)
        {
            try
            {
                var response = await _httpClient.PostAsync("/api/user/savenewuser", JsonContent.Create(userDto));

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var newUserDto = System.Text.Json.JsonSerializer.Deserialize<UserDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newUserDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось зарегистрировать нового польхователя. {ex.Message}");
                throw new SoupiException("Не удалось зарегистрировать нового польхователя. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        } 

        public async Task<UserDto?> GetUserByLogin(string login)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/User/GetUserByLogin?login={login}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                var newContent = await response.Content.ReadAsStringAsync();

                var newUserDto = System.Text.Json.JsonSerializer.Deserialize<UserDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newUserDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о текущем пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о текущем пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            } 
        }
    }
}
