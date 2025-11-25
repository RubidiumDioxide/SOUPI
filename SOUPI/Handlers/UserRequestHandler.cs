using SOUPIShared.Exceptions;
using SOUPIShared.Dtos;
using System.Text.Json;
using System.Net; 


namespace SOUPICore.Services
{
    public class UserRequestHandler 
    {
        private readonly ILogger<UserRequestHandler> _logger;
        private readonly HttpClient _httpClient;

        private const string createUrl = "/api/user/create"; 
        private const string getByLoginUrl = "/api/user/getbylogin/";

        public UserRequestHandler(ILogger<UserRequestHandler> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient; 
        }

        public async Task<UserDto> Create(UserDto userDto)
        {
            try
            {
                var response = await _httpClient.PostAsync(createUrl, JsonContent.Create(userDto));

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

        public async Task<UserDto?> GetByLogin(string login)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByLoginUrl}{login}");

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                }

                response.EnsureSuccessStatusCode(); 

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
