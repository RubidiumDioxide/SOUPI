using System.ComponentModel.DataAnnotations; 


namespace SOUPIShared.Models
{
    public class Assignment
    {
        public Guid Id { get; set; }

        [Required]
        public Guid TeamMemberId { get; set; } = default!;

        [Required] 
        public Guid JobId { get; set; } = default!;

        [MaxLength(100, ErrorMessage = "Комментарий слишком длинный (максимум 100 символов)")]
        public string? Comment { get; set; }


        public virtual TeamMember TeamMember { get; set; } = default!; 
        public virtual Job Job { get; set; } = default!;    
    }
}
