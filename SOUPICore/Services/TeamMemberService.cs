using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Resources;


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

        public async Task<TeamMemberDisplayDto> GetById(Guid teamMemberId, CancellationToken ct = default)
        {
            try
            {
                var teamMember = await _context.TeamMembers.FindAsync([teamMemberId], cancellationToken: ct); 

                if(teamMember == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.TeamMemberNotFound);
                }

                return new TeamMemberDisplayDto(teamMember);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<TeamMemberDisplayDto>> GetByJobId(Guid jobId, CancellationToken ct = default)
        {
            try
            {
                var job = await _context.Jobs.FindAsync([jobId], cancellationToken: ct); 

                if(job == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.JobNotFound);
                }

                var teamMembers = await _context.TeamMembers
                    .Where(tm => tm.ProjectId == job.ProjectId)
                    .ToListAsync(ct);

                return teamMembers.Select(tm => new TeamMemberDisplayDto(tm));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<TeamMemberDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                var project = await _context.Projects.FindAsync([projectId], cancellationToken: ct);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                var teamMembers = await _context.TeamMembers
                    .Where(tm => tm.ProjectId == projectId)
                    .ToListAsync(ct);

                return teamMembers.Select(tm => new TeamMemberDisplayDto(tm)); 
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw; 
            }
        }

        public async Task<TeamMemberDto> Update(TeamMemberDto teamMemberDto, CancellationToken ct = default)
        {
            try
            {
                var existingTeamMember = await _context.TeamMembers.FirstOrDefaultAsync(tm => tm.UserId == teamMemberDto.UserId && tm.ProjectId == teamMemberDto.ProjectId, ct);

                if (existingTeamMember == null)
                {
                    throw new BadRequestException($"Невозмжоно изменить роль участника команды, т.к. этого участника команды нет в системе ");
                }

                existingTeamMember.Role = teamMemberDto.Role;
                existingTeamMember.SupervisorId = teamMemberDto.SupervisorId; 

                await _context.SaveChangesAsync(ct);

                return new TeamMemberDto(existingTeamMember); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteById(Guid id, CancellationToken ct = default)
        {
            try
            {
                var teamMember = await _context.TeamMembers.FindAsync([id], cancellationToken: ct); 

                if(teamMember == null)
                {
                    throw new BadRequestException("Участника команды нельзя исключить, т.к. он не найден в системе ");
                }

                if(teamMember.UserId == teamMember.Project.CreatorId)
                {
                    throw new BadRequestException("Создателя пректа нельзя исключить из команды ");
                }

                var jobs = teamMember.CreatedJobs.ToList();
                var createdAssignments = jobs.SelectMany(j => j.Assignments).ToList(); 
                var createdActivities = createdAssignments.SelectMany(a => a.Activities).ToList(); 
                var assignments = teamMember.Assignments.ToList();
                var activities = assignments.SelectMany(a => a.Activities).ToList();

                _context.Activities.RemoveRange(activities); 
                _context.Assignments.RemoveRange(assignments);
                _context.Activities.RemoveRange(createdActivities);
                _context.Assignments.RemoveRange(createdAssignments);
                _context.Jobs.RemoveRange(jobs);
                _context.TeamMembers.Remove(teamMember);

                await _context.SaveChangesAsync(ct); 
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message); 
                throw; 
            }
        }
    }
}
