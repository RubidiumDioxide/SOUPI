using SOUPI.Handlers.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using System.Text.Json;
using System.Net;


namespace SOUPI.Handlers
{
    public class NotificationRequestHandler : INotificationRequestHandler
    {
        private readonly ILogger<NotificationRequestHandler> _logger;
        private readonly HttpClient _httpClient;

        private const string createUrl = "/api/Notification/create";
        private const string getByReceiverIdUrl = "/api/Notification/getbyreceiverid/";

        public NotificationRequestHandler(ILogger<NotificationRequestHandler> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<NotificationDisplayDto>> GetByReceiverId(Guid receiverId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{getByReceiverIdUrl}{receiverId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var NotificationDtos = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<NotificationDisplayDto>>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return NotificationDtos!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить уведомления {ex.Message}");
                throw new SoupiException("Не удалось загрузить уведомления. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<NotificationDto> Create(NotificationDto NotificationDto)
        {
            try
            {
                var response = await _httpClient.PostAsync(createUrl, JsonContent.Create(NotificationDto));

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var newNotificationDto = System.Text.Json.JsonSerializer.Deserialize<NotificationDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return newNotificationDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось создать уведомление. {ex.Message}");
                throw new SoupiException("Не удалось создать уведомление. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }
    }
}
