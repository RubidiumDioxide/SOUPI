using SOUPIShared.Dtos; 


namespace SOUPI.Handlers.Interfaces
{
    public interface ITeamMemberRequestHandler 
    {
        public Task<IEnumerable<TeamMemberDisplayDto>> GetByProjectId(Guid projectId);

        public Task<TeamMemberDto> Create(TeamMemberDto newTeamMember);

        public Task<TeamMemberDto> Update(TeamMemberDto teamMemberDto);

        public Task DeleteById(Guid id); 
    }
}
