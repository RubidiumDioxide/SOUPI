using SOUPI.Handlers.Interfaces;
using SOUPIShared.Exceptions;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPICore.Services.Interfaces; 


namespace SOUPI.Handlers
{
    public class NotificationRequestHandler : INotificationRequestHandler
    {
        private readonly ILogger<NotificationRequestHandler> _logger;
        private readonly INotificationService _notificationService;

        public NotificationRequestHandler(ILogger<NotificationRequestHandler> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService; 
        }

        public async Task<IEnumerable<NotificationDisplayDto>> GetByReceiverId(Guid receiverId, CancellationToken ct = default)
        {
            try
            {
                return await _notificationService.GetByReceiverId(receiverId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить уведомления {ex.Message}");
                throw new SoupiException("Не удалось загрузить уведомления. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<NotificationDto> Create(NotificationDto NotificationDto, CancellationToken ct = default)
        {
            try
            {
                return await _notificationService.Create(NotificationDto, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось создать уведомление. {ex.Message}");
                throw new SoupiException("Не удалось создать уведомление. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<NotificationDto> AcceptInvite(Guid notificationId, CancellationToken ct = default)
        {
            try
            {
                return await _notificationService.AcceptInvite(notificationId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось принять приглашение: {ex.Message}");
                throw new SoupiException("Не удалось принять приглашение. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<NotificationDto> MarkAsViewed(Guid notificationId, CancellationToken ct = default)
        {
            try
            {
                return await _notificationService.MarkAsViewed(notificationId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось отметить уведомление как прочитанное: {ex.Message}");
                throw new SoupiException("Не удалось отметить уведомление как прочитанное. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }
    }
}
