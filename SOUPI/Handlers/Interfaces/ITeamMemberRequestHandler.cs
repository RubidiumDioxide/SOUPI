using SOUPIShared.Dtos; 


namespace SOUPI.Handlers.Interfaces
{
    public interface ITeamMemberRequestHandler 
    {
        public Task<IEnumerable<TeamMemberDisplayDto>> GetByProjectId(Guid projectId);

        public Task<TeamMemberDisplayDto> Create(TeamMemberDto newTeamMember);

        public Task<TeamMemberDisplayDto> UpdateRole(TeamMemberDto teamMemberDto);

        public Task<TeamMemberDisplayDto> UpdateSupervisor(TeamMemberDto teamMemberDto);

        public Task DeleteById(Guid id);
    }
}
