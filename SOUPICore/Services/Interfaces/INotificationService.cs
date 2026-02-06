using SOUPIShared.Dtos;

namespace SOUPICore.Services.Interfaces
{
    public interface INotificationService
    {
        public Task<IEnumerable<NotificationDisplayDto>> GetByReceiverId(Guid receiverId);

        public Task<NotificationDto> Create(NotificationDto newNotification);

        public Task<NotificationDto> AcceptInvite(Guid notificationId);

        public Task<NotificationDto> MarkAsViewed (Guid notificationId); 
    }
}
