using System.ComponentModel.DataAnnotations;
using SOUPIShared.Attributes; 


namespace SOUPIShared.Models
{
    public class Activity
    {   
        public Guid Id { get; set; }

        [Required]
        public Guid AssignmentId { get; set; }

        [ValidCommitHash]
        public string? Commit { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Комментарий слишком длинный (максимум 255 символов)")]
        [ConsistsOfNumbersCyrillicLatin]
        public string Comment { get; set; } = default!; 

        public DateTime CreationDateTime { get; set; } = default;


        public virtual Assignment Assignment { get; set; } = default!; 
    }
}
