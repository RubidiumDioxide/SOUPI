using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces
{
    public interface INotificationRequestHandler
    {
        public Task<IEnumerable<NotificationDisplayDto>> GetByReceiverId(Guid receiverId, CancellationToken ct = default); 
        
        public Task<NotificationDto> Create(NotificationDto notificationDto, CancellationToken ct = default); 
        
        public Task<NotificationDto> AcceptInvite(Guid notificationId, CancellationToken ct = default); 
        
        public Task<NotificationDto> MarkAsViewed(Guid notificationId, CancellationToken ct = default); 
    }
}
