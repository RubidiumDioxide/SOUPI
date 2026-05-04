using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces
{
    public interface ITeamMemberRequestHandler 
    {
        public Task<TeamMemberDisplayDto> GetById(Guid teamMemberId);

        public Task<IEnumerable<TeamMemberDisplayDto>> GetByProjectId(Guid projectId);

        public Task<TeamMemberDto> Create(TeamMemberDto newTeamMember);

        public Task<TeamMemberDto> Update(TeamMemberDto teamMemberDto);

        public Task Delete(Guid id); 
    }
}
