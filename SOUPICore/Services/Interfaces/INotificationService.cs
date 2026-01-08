using SOUPIShared.Dtos;

namespace SOUPICore.Services.Interfaces
{
    public interface INotificationService
    {
        // create 
        public Task<NotificationDto> Create(NotificationDto newNotification); 
    }
}
