using SOUPIShared.Exceptions;
using SOUPIShared.Dtos;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities; 


namespace SOUPI.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly HttpClient _httpClient;

        private const string createUrl = "/api/user/create"; 
        private const string getUrl = "/api/user/getbylogin/";

        public UserService(ILogger<UserService> logger, HttpClient httpClient)
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
                var queryParams = new Dictionary<string, string>
                {
                    ["login"] = login
                };
                var requestUrl = QueryHelpers.AddQueryString(getUrl, queryParams);
                
                var response = await _httpClient.GetAsync(requestUrl); 

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
