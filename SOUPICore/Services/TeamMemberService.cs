using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos;
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

        public async Task<TeamMemberDisplayDto> GetById(Guid teamMemberId)
        {
            try
            {
                var teamMember = await _context.TeamMembers.FindAsync(teamMemberId); 

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

        public async Task<IEnumerable<TeamMemberDisplayDto>> GetByJobId(Guid jobId)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(jobId); 
                
                if(job == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.JobNotFound);
                }

                var teamMembers = await _context.TeamMembers
                    .Where(tm => tm.ProjectId == job.ProjectId)
                    .ToListAsync();

                return teamMembers.Select(tm => new TeamMemberDisplayDto(tm));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<TeamMemberDisplayDto>> GetByProjectId(Guid projectId)
        {
            try
            {
                var project = await _context.Projects.FindAsync(projectId);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

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
