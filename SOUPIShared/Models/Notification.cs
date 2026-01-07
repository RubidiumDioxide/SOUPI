using System.ComponentModel.DataAnnotations; 


namespace SOUPIShared.Models
{
    public class Notification
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Поле сообщения обязательное")]
        [MaxLength(255, ErrorMessage = "Сообщение слишком длинное (максимум 255 символов)")]
        [MinLength(1, ErrorMessage = "Сообщение слишком короткое (минимум 1 символ)")]
        public string Message { get; set; } = default!;

        [Required]
        public Guid SenderId { get; set; } = default!; 

        [Required]
        public Guid ReceiverId { get; set; } = default!;

        [Required]
        public Guid ProjectId { get; set; } = default!;  

        [Required]
        public NotificationType NotificationType { get; set; } = NotificationType.Info;


        public virtual Project Project { get; set; } = default!;
        public virtual User Sender { get; set; } = default!;
        public virtual User Receiver { get; set; } = default!;
    }
}
