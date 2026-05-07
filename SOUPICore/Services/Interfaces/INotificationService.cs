using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPICore.Services.Interfaces
{
    public interface INotificationService
    {
        public Task<IEnumerable<NotificationDisplayDto>> GetByReceiverId(Guid receiverId, CancellationToken ct = default);

        public Task<NotificationDto> Create(NotificationDto newNotification, CancellationToken ct = default);

        public Task<NotificationDto> AcceptInvite(Guid notificationId, CancellationToken ct = default);

        public Task<NotificationDto> MarkAsViewed (Guid notificationId, CancellationToken ct = default); 
    }
}
