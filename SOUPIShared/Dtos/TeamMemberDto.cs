using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class TeamMemberDto
    {
        public int Id { get; set; }

        public int UsedId { get; set; }

        public int ProjectId { get; set; }

        public string? Role { get; set; }

        public int? ManagerId { get; set; }

        public TeamMemberDto(TeamMember teamMember)
        {
            Id = teamMember.Id; 
            UsedId = teamMember.UsedId; 
            ProjectId = teamMember.ProjectId; 
            Role = teamMember.Role;
            ManagerId = teamMember.ManagerId; 
        }

        public TeamMemberDto() { }
    }
}
