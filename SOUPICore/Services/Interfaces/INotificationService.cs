using SOUPIShared.Dtos;

namespace SOUPICore.Services.Interfaces
{
    public interface INotificationService
    {
        public Task<IEnumerable<NotificationDisplayDto>> GetByReceiverId(Guid id);

        public Task<NotificationDto> Create(NotificationDto newNotification);

        // мб убрать потом, как будто не должны удаляться 
        public Task DeleteById(Guid id); 
    }
}
