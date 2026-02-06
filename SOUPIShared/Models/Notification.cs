using SOUPIShared.Misc;
using System.ComponentModel.DataAnnotations; 


namespace SOUPIShared.Models
{
    public class Notification
    {
        public Guid Id { get; set; }

        [MaxLength(255, ErrorMessage = "Сообщение слишком длинное (максимум 255 символов)")]
        public string Message { get; set; } = default!;

        [Required]
        public Guid SenderId { get; set; } = default!; 

        [Required]
        public Guid ReceiverId { get; set; } = default!;

        [Required]
        public Guid ProjectId { get; set; } = default!;  

        [Required]
        public NotificationType NotificationType { get; set; } = NotificationType.Info;

        [MaxLength(255, ErrorMessage = "Роль слишком длинная (максимум 255 символов)")]
        public string? Role { get; set; } = null; 

        [Required]
        public bool HasBeenViewed { get; set; } = false; 


        public virtual Project Project { get; set; } = default!;
        public virtual User Sender { get; set; } = default!;
        public virtual User Receiver { get; set; } = default!;
    }
}
