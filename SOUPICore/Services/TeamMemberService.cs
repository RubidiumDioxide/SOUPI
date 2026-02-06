using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions; 


namespace SOUPICore.Services
{
    public class TeamMemberService : ITeamMemberService 
    {
        private readonly ILogger<TeamMemberService> _logger;
        private readonly SoupiDbContext _context;

        public TeamMemberService(ILogger<TeamMemberService> logger, SoupiDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<TeamMemberDisplayDto>> GetByProjectId(Guid projectId)
        {
            try
            {
                var teamMembers = await _context.TeamMembers
                    .Where(tm => tm.ProjectId == projectId)
                    .ToListAsync();

                return teamMembers.Select(tm => new TeamMemberDisplayDto(tm)); 
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw; 
            }
        }
        public async Task<TeamMemberDto> Update(TeamMemberDto teamMemberDto)
        {
            try
            {
                var existingTeamMember = await _context.TeamMembers.FirstOrDefaultAsync(tm => tm.UserId == teamMemberDto.UserId && tm.ProjectId == teamMemberDto.ProjectId);

                if (existingTeamMember == null)
                {
                    throw new BadRequestException($"Невозмжоно изменить роль участника команды, т.к. этого участника команды нет в системе ");
                }

                existingTeamMember.Role = teamMemberDto.Role;
                existingTeamMember.SupervisorId = teamMemberDto.SupervisorId; 

                await _context.SaveChangesAsync();

                return new TeamMemberDto(existingTeamMember); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteById(Guid id)
        {
            try
            {
                var teamMember = await _context.TeamMembers.FindAsync(id); 

                if(teamMember == null)
                {
                    throw new BadRequestException("Участника команды нельзя исключить, т.к. он не найден в системе ");
                }

                if(teamMember.UserId == teamMember.Project.CreatorId)
                {
                    throw new BadRequestException("Создателя пректа нельзя исключить из команды ");
                }

                _context.TeamMembers.Remove(teamMember);
                await _context.SaveChangesAsync(); 
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message); 
                throw; 
            }
        }
    }
}
