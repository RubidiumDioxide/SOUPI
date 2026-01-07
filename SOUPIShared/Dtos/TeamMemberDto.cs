using System.ComponentModel.DataAnnotations; 
using SOUPIShared.Models;


namespace SOUPIShared.Dtos
{
    public class TeamMemberDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; } = default!;

        [Required]
        public Guid ProjectId { get; set; } = default!;

        [MaxLength(255, ErrorMessage = "Роль слишком длинная (максимум 255 символов)")]
        public string? Role { get; set; } = null;

        public Guid? SupervisorId { get; set; } = null;


        public TeamMemberDto(TeamMember teamMember)
        {
            Id = teamMember.Id; 
            UserId = teamMember.UserId; 
            ProjectId = teamMember.ProjectId; 
            Role = teamMember.Role; 
            SupervisorId = teamMember.SupervisorId; 
        }

        public TeamMemberDto() { } 
    }
}
