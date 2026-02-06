using SOUPIShared.Misc;
using SOUPIShared.Models;
using System.ComponentModel.DataAnnotations;

namespace SOUPIShared.Dtos
{
    public class NotificationDisplayDto
    {
        public Guid Id { get; set; }

        [MaxLength(255, ErrorMessage = "Сообщение слишком длинное (максимум 255 символов)")]
        public string Message { get; set; } = default!;

        [Required]
        public Guid SenderId { get; set; } = default!;

        [Required]
        public string SenderLogin { get; set; } = default!; 

        [Required]
        public Guid ReceiverId { get; set; } = default!;
        
        [Required]
        public string ReceiverLogin { get; set; } = default!;

        [Required]
        public Guid ProjectId { get; set; } = default!;

        [Required]
        public string ProjectName { get; set; } = default!;

        [Required]
        public NotificationType NotificationType { get; set; } = NotificationType.Info;

        [MaxLength(255, ErrorMessage = "Роль слишком длинная (максимум 255 символов)")]
        public string? Role { get; set; } = null;

        [Required]
        public bool HasBeenViewed { get; set; } = false;
        
        [Required]
        public DateTime CreationDateTime { get; set; } = default!;
        
        public NotificationDisplayDto(Notification notification)
        {
            Id = notification.Id; 
            Message = notification.Message; 
            SenderId = notification.SenderId;
            SenderLogin = notification.Sender.Login; 
            ReceiverId = notification.ReceiverId; 
            ReceiverLogin = notification.Receiver.Login; 
            ProjectId = notification.ProjectId;
            ProjectName = notification.Project.Name; 
            NotificationType = notification.NotificationType; 
            Role = notification.Role; 
            HasBeenViewed = notification.HasBeenViewed; 
            CreationDateTime = notification.CreationDateTime; 
        }

        public NotificationDisplayDto() { }
    }
}
