using SOUPIShared.Attributes;
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
        [ValidGitHubUsername]
        public string UserLogin { get; set; } = default!; 

        [Required]
        public Guid ProjectId { get; set; } = default!;

        [Required]
        [ConsistsOfNumbersCyrillicLatin]
        public string ProjectTitle { get; set; } = default!;  

        [MaxLength(255, ErrorMessage = "Роль слишком длинная (максимум 255 символов)")]
        [ConsistsOfNumbersCyrillicLatin]
        public string? Role { get; set; } = null;

        public Guid? SupervisorId { get; set; } = null;

        [ValidGitHubUsername] 
        public string? SupervisorLogin { get; set; }


        public TeamMemberDisplayDto(TeamMember teamMember)
        {
            Id = teamMember.Id; 
            UserId = teamMember.UserId;
            UserLogin = teamMember.User.Login; 
            ProjectId = teamMember.ProjectId; 
            ProjectTitle = teamMember.Project.Title; 
            Role = teamMember.Role; 
            SupervisorId = teamMember.SupervisorId;
            SupervisorLogin = teamMember.Supervisor?.User.Login; 
        }

        public TeamMemberDisplayDto() { } 
    }
}
