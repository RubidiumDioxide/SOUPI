using SOUPIShared.Dtos; 


namespace SOUPICore.Services.Interfaces
{
    public interface ITeamMemberService
    {
        // get by self id 
        //public Task<TeamMemberDto> GetById(Guid id); 

        // get by composite user-project id 
        //public Task<TeamMemberDto> GetByUserProjectId(Guid userId, Guid projectId);

        // get by user id 
        //public Task<IEnumerable<TeamMemberDto>> GetByUserId(Guid userId);

        // get by project id 
        public Task<IEnumerable<TeamMemberDisplayDto>> GetByProjectId(Guid projectId);

        // create 
        public Task<TeamMemberDisplayDto> Create(TeamMemberDto newTeamMember);

        // change role 
        public Task<TeamMemberDisplayDto> UpdateRole(TeamMemberDto teamMemberDto);

        // change supervisor 
        public Task<TeamMemberDisplayDto> UpdateSupervisor(TeamMemberDto teamMemberDto);

        // delete by self id
        public Task DeleteById(Guid id);
    }
}
