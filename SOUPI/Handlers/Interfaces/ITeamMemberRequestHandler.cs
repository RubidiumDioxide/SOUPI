using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces
{
    public interface ITeamMemberRequestHandler 
    {
        public Task<TeamMemberDisplayDto> GetById(Guid teamMemberId, CancellationToken ct = default);

        public Task<IEnumerable<TeamMemberDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default);

        public Task<TeamMemberDto> Create(TeamMemberDto newTeamMember, CancellationToken ct = default);

        public Task<TeamMemberDto> Update(TeamMemberDto teamMemberDto, CancellationToken ct = default);

        public Task Delete(Guid id, CancellationToken ct = default); 
    }
}
