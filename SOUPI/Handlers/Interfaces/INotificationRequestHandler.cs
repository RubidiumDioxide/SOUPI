using SOUPIShared.Dtos;


namespace SOUPI.Handlers.Interfaces
{
    public interface INotificationRequestHandler
    {
        public Task<IEnumerable<NotificationDisplayDto>> GetByReceiverId(Guid receiverId); 
        
        public Task<NotificationDto> Create(NotificationDto notificationDto); 
        
        public Task<NotificationDto> AcceptInvite(Guid notificationId); 
        
        public Task<NotificationDto> MarkAsViewed(Guid notificationId); 
    }
}
