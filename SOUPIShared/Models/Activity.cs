using System.ComponentModel.DataAnnotations; 


namespace SOUPIShared.Models
{
    public class Activity
    {   
        public Guid Id { get; set; }

        [Required]
        public Guid AssignmentId { get; set; } 

        [MaxLength(40, ErrorMessage = "Длина хэша коммита - 40 символов")]
        [MinLength(40, ErrorMessage = "Длина хэша коммита - 40 символов")]
        public string? Commit { get; set; }

        [MaxLength(255, ErrorMessage = "Комментарий слишком длинный (максимум 255 символов)")]
        public string? Comment { get; set; }


        public virtual Assignment Assignment { get; set; } = default!; 
    }
}
