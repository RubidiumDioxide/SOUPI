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

        [MaxLength(255, ErrorMessage = "Комментарий слишком длинный (максимум 255 символов)")]
        [ConsistsOfNumbersCyrillicLatin]
        public string? Comment { get; set; }


        public virtual Assignment Assignment { get; set; } = default!; 
    }
}
