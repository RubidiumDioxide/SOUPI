using SOUPIShared.Misc;
using SOUPIShared.Models;
using System.ComponentModel.DataAnnotations;

namespace SOUPIShared.Dtos
{
    public class NotificationDisplayDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Поле сообщения обязательное")]
        [MaxLength(255, ErrorMessage = "Сообщение слишком длинное (максимум 255 символов)")]
        [MinLength(1, ErrorMessage = "Сообщение слишком короткое (минимум 1 символ)")]
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
        
        [Required]
        public bool HasBeenViewed { get; set; } = false;

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
            HasBeenViewed = notification.HasBeenViewed; 
        }

        public NotificationDisplayDto() { }
    }
}
