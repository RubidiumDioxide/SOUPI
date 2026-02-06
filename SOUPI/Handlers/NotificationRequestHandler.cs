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

        private const string getByReceiverIdUrl = "/api/Notification/getbyreceiverid/";
        private const string createUrl = "/api/Notification/create/";
        private const string acceptInviteUrl = "/api/Notification/acceptinvite/";
        private const string markAsViewedUrl = "/api/Notification/markasviewed/";

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

        public async Task<NotificationDto> AcceptInvite(Guid notificationId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{acceptInviteUrl}{notificationId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var notificationDto = System.Text.Json.JsonSerializer.Deserialize<NotificationDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return notificationDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось принять приглашение: {ex.Message}");
                throw new SoupiException("Не удалось принять приглашение. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<NotificationDto> MarkAsViewed(Guid notificationId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{markAsViewedUrl}{notificationId}");

                response.EnsureSuccessStatusCode();

                var newContent = await response.Content.ReadAsStringAsync();

                var notificationDto = System.Text.Json.JsonSerializer.Deserialize<NotificationDto>(newContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return notificationDto!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось отметить уведомление как прочитанное: {ex.Message}");
                throw new SoupiException("Не удалось отметить уведомление как прочитанное. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }
    }
}
