using System.ComponentModel.DataAnnotations;

namespace SOUPIShared.Models
{
    public class TeamMember
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; } = default!;

        [Required]
        public Guid ProjectId { get; set; } = default!; 

        [MaxLength(255, ErrorMessage = "Роль слишком длинная (максимум 255 символов)")]
        public string? Role { get; set; } = null;

        public Guid? SupervisorId { get; set; } = null;
       

        public virtual User User { get; set; } = default!;
        public virtual Project Project { get; set; } = default!; 
        public virtual TeamMember? Supervisor { get; set; } 
        public virtual List<TeamMember> Subservient { get; set; } = default!;
        public virtual List<Assignment> Assignments { get; set; } = default!;  
    }
}
