using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPICore.Services.Interfaces
{
    public interface ITeamMemberService
    {
        public Task<TeamMemberDisplayDto> GetById(Guid teamMemberId, CancellationToken ct = default);

        public Task<IEnumerable<TeamMemberDisplayDto>> GetByJobId(Guid jobId, CancellationToken ct = default);

        public Task<IEnumerable<TeamMemberDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default);

        public Task<TeamMemberDto> Update(TeamMemberDto teamMemberDto, CancellationToken ct = default);

        public Task Delete(Guid id, CancellationToken ct = default);
    }
}
