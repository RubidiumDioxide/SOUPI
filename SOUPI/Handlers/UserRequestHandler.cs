using SOUPIShared.Exceptions;
using System.Text.Json;
using System.Net;
using SOUPI.Handlers.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers
{
    public class UserRequestHandler : IUserRequestHandler
    {
        private readonly ILogger<UserRequestHandler> _logger;
        private readonly HttpClient _httpClient;

        private const string createUrl = "/api/user/create"; 
        private const string getUrl = "/api/user/get/";
        private const string getByIdUrl = "/api/user/getbyid/";
        private const string getByLoginUrl = "/api/user/getbylogin/";

        public UserRequestHandler(ILogger<UserRequestHandler> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient; 
        }

        public async Task<UserDto> Create(UserDto userDto, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.PostAsync(createUrl, JsonContent.Create(userDto), ct);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

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

        public async Task<IEnumerable<UserDto>> Get(CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getUrl}", ct);

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

                var newUserDto = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<UserDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newUserDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<UserDto?> GetById(Guid id, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByIdUrl}{id}", ct);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                }

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync(ct);

                var newUserDto = System.Text.Json.JsonSerializer.Deserialize<UserDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newUserDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<UserDto?> GetByLogin(string login, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByLoginUrl}{login}", ct);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null; 
                    }
                }

                response.EnsureSuccessStatusCode(); 

                var newContent = await response.Content.ReadAsStringAsync(ct);

                var newUserDto = System.Text.Json.JsonSerializer.Deserialize<UserDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newUserDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            } 
        }
    }
}
