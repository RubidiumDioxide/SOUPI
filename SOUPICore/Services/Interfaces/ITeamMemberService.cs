using SOUPIShared.Dtos; 


namespace SOUPICore.Services.Interfaces
{
    public interface ITeamMemberService
    {
        public Task<IEnumerable<TeamMemberDisplayDto>> GetByProjectId(Guid projectId);
        
        public Task<TeamMemberDto> Update(TeamMemberDto teamMemberDto);

        public Task DeleteById(Guid id);
    }
}
