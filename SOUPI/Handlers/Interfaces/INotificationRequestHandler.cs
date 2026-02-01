using SOUPIShared.Dtos;


namespace SOUPI.Handlers.Interfaces
{
    public interface INotificationRequestHandler
    {
        public Task<IEnumerable<NotificationDisplayDto>> GetByReceiverId(Guid receiverId); 
        
        public Task<NotificationDto> Create(NotificationDto notificationDto); 
    }
}
