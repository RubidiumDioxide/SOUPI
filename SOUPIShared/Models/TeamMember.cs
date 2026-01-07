using System.ComponentModel.DataAnnotations;

namespace SOUPIShared.Models
{
    public class TeamMember
    {
        public Guid UserId { get; set; } 
        public Guid ProjectId { get; set; }

        public string? Role { get; set; } = null;

        public Guid? SupervisorUserId { get; set; } = null;
        public Guid? SupervisorProjectId { get; set; } = null;
       

        public virtual User User { get; set; } = default!;
        public virtual Project Project { get; set; } = default!; 
        public virtual TeamMember? Supervisor { get; set; } 
        public virtual List<TeamMember> Subservient { get; set; } = default!;   
    }
}
