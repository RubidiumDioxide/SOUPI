using Microsoft.Extensions.Logging;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Models;
using Microsoft.EntityFrameworkCore;
using SOUPICore.Services.Interfaces;


namespace SOUPICore.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly SoupiDbContext _context;

        public NotificationService(ILogger<NotificationService> logger, SoupiDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<NotificationDisplayDto>> GetByReceiverId(Guid receiverId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.ReceiverId == receiverId)
                    .ToListAsync();

                return notifications.Select(p => new NotificationDisplayDto(p));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<NotificationDto> Create(NotificationDto newNotificationDto)
        {
            try
            {
                var newNotification = new Notification()
                {
                    Message = newNotificationDto.Message,
                    SenderId = newNotificationDto.SenderId,
                    ReceiverId = newNotificationDto.ReceiverId,
                    ProjectId = newNotificationDto.ProjectId,
                    NotificationType = newNotificationDto.NotificationType,
                    HasBeenViewed = false
                }; 

                await _context.Notifications.AddAsync(newNotification);
                await _context.SaveChangesAsync(); 

                return new NotificationDto(newNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteById(Guid id)
        {
            try
            {
                var Notification = await _context.Notifications.FindAsync(id);

                if (Notification == null)
                {
                    throw new BadRequestException("Уведомление нельзя удалить, т.к. он не найден в системе ");
                }

                _context.Notifications.Remove(Notification);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
