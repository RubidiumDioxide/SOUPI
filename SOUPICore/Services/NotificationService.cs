using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Misc;
using SOUPIShared.Models;


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

        public async Task<IEnumerable<NotificationDisplayDto>> GetByReceiverId(Guid receiverId, CancellationToken ct = default)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.ReceiverId == receiverId)
                    .ToListAsync(ct);

                return notifications.Select(p => new NotificationDisplayDto(p));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<NotificationDto> Create(NotificationDto newNotificationDto, CancellationToken ct = default)
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
                    Role = newNotificationDto.Role, 
                    HasBeenViewed = false
                }; 

                await _context.Notifications.AddAsync(newNotification, ct);
                await _context.SaveChangesAsync(ct); 

                return new NotificationDto(newNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<NotificationDto> AcceptInvite(Guid notificationId, CancellationToken ct = default)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync([notificationId], cancellationToken: ct);

                if (notification == null)
                {
                    throw new BadRequestException($"Невозмжоно добавить пользователя в команду проекта, т. к. такого приглашения не существует");
                }

                if (notification.NotificationType != NotificationType.Invitation)
                {
                    throw new BadRequestException($"Невозмжоно добавить пользователя в команду проекта, т. к. тип найденного приглашения неверный");
                }

                var user = await _context.Users.FindAsync([notification.ReceiverId], cancellationToken: ct);
                var project = await _context.Projects.FindAsync([notification.ProjectId], cancellationToken: ct);

                if (user == null || project == null)
                {
                    throw new BadRequestException($"Невозмжоно добавить пользователя в команду проекта, т. к. такого проекта и/или пользователя не существует");
                }

                var existingTeamMember = await _context.TeamMembers.FirstOrDefaultAsync(tm => tm.UserId == notification.ReceiverId && tm.ProjectId == notification.ProjectId, ct);

                if (existingTeamMember != null)
                {
                    throw new BadRequestException($"Невозмжоно добавить в команду проекта {project.Title} пользователя {user.Login}, т.к. этот пользователь уже есть в команде ");
                }

                var supervisor = project.TeamMembers.FirstOrDefault(tm => tm.UserId == project.CreatorId);

                if (supervisor == null)
                {
                    throw new BadRequestException($"Ошибка при вычислении руководителя в команде проекта {project.Title} для пользователя {user.Login} ");
                }

                var newTeamMember = new TeamMember()
                {
                    UserId = notification.ReceiverId,
                    ProjectId = notification.ProjectId,
                    Role = notification.Role,
                    SupervisorId = supervisor.Id
                };

                await _context.TeamMembers.AddAsync(newTeamMember, ct);

                notification.HasBeenViewed = true;

                await _context.SaveChangesAsync(ct);

                return new NotificationDto(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<NotificationDto> MarkAsViewed(Guid notificationId, CancellationToken ct = default)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync([notificationId], cancellationToken: ct);

                if (notification == null)
                {
                    throw new BadRequestException($"Уведомление не найдено");
                }

                if (!notification.HasBeenViewed)
                {
                    notification.HasBeenViewed = true;
                    await _context.SaveChangesAsync(ct);
                }

                return new NotificationDto(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
