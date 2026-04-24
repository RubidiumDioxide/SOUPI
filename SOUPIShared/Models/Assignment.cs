using System.ComponentModel.DataAnnotations;
using SOUPIShared.Attributes; 


namespace SOUPIShared.Models
{
    public class Assignment
    {
        public Guid Id { get; set; }

        [Required]
        public Guid TeamMemberId { get; set; } = default!;

        [Required] 
        public Guid JobId { get; set; } = default!;

        [MaxLength(255, ErrorMessage = "Комментарий слишком длинный (максимум 255 символов)")]
        [ConsistsOfNumbersCyrillicLatin]
        public string? Comment { get; set; }


        public virtual TeamMember TeamMember { get; set; } = default!; 
        public virtual Job Job { get; set; } = default!;
        public virtual List<Activity> Activities { get; set; } = default!; 
    }
}
