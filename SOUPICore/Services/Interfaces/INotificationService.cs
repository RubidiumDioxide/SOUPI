using SOUPIShared.Dtos;

namespace SOUPICore.Services.Interfaces
{
    public interface INotificationService
    {
        public Task<IEnumerable<NotificationDisplayDto>> GetByReceiverId(Guid id);

        public Task<NotificationDto> Create(NotificationDto newNotification);
    }
}
