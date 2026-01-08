using SOUPIShared.Models;
using System.ComponentModel.DataAnnotations; 


namespace SOUPIShared.Dtos
{
    public class TeamMemberDisplayDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; } = default!;

        [Required]
        public string UserLogin { get; set; } = default!; 

        [Required]
        public Guid ProjectId { get; set; } = default!;

        [Required]
        public string ProjectName { get; set; } = default!;  

        [MaxLength(255, ErrorMessage = "Роль слишком длинная (максимум 255 символов)")]
        public string? Role { get; set; } = null;

        public Guid? SupervisorId { get; set; } = null;

        public string? SupervisorName { get; set; } 
        
    
        public TeamMemberDisplayDto(TeamMember teamMember)
        {
            Id = teamMember.Id; 
            UserId = teamMember.UserId;
            UserLogin = teamMember.User.Login; 
            ProjectId = teamMember.ProjectId; 
            ProjectName = teamMember.Project.Name; 
            Role = teamMember.Role; 
            SupervisorId = teamMember.SupervisorId;
            SupervisorName = teamMember.Supervisor?.User.Login;  
        }

        public TeamMemberDisplayDto() { } 
    }
}
