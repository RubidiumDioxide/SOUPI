using SOUPIShared.Models;
using SOUPIShared.Dtos; 


namespace SOUPIShared.TestData
{
    public static class TeamMembers
    {
        private static Guid projectId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        private static TeamMemberDisplayDto pm = new TeamMemberDisplayDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(), 
            UserLogin = "Lea", 
            ProjectId = projectId,
            Role = "Project Manager"
        };

        private static TeamMemberDisplayDto techLead = new TeamMemberDisplayDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            UserLogin = "Kat",
            ProjectId = projectId,
            Role = "Tech Lead",
            SupervisorId = pm.Id
        };

        private static TeamMemberDisplayDto seniorDev = new TeamMemberDisplayDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            UserLogin = "Mansur",
            ProjectId = projectId,
            Role = "Senior Developer",
            SupervisorId = pm.Id
        };

        private static TeamMemberDisplayDto junior1 = new TeamMemberDisplayDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            UserLogin = "Ilhamich",
            ProjectId = projectId,
            Role = "Junior Developer",
            SupervisorId = techLead.Id
        };

        private static TeamMemberDisplayDto junior2 = new TeamMemberDisplayDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            UserLogin = "Aidar",
            ProjectId = projectId,
            Role = "Junior Developer",
            SupervisorId = techLead.Id
        };

        private static TeamMemberDisplayDto qa = new TeamMemberDisplayDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            UserLogin = "Katrin",
            ProjectId = projectId,
            Role = "QA Engineer",
            SupervisorId = seniorDev.Id
        };

        private static TeamMemberDisplayDto devops = new TeamMemberDisplayDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            UserLogin = "Danil",
            ProjectId = projectId,
            Role = "DevOps Engineer",
            SupervisorId = pm.Id
        };

        public static List<TeamMemberDisplayDto> testTeamMembers = new List<TeamMemberDisplayDto>
        {
            pm, techLead, seniorDev, junior1, junior2, qa, devops
        };
    }
}
