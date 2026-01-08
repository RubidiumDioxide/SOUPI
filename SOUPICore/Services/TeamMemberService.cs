using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions; 
using SOUPIShared.Models; 


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

        // create 
        public async Task<TeamMemberDisplayDto> Create(TeamMemberDto newTeamMemberDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(newTeamMemberDto.UserId);
                var project = await _context.Projects.FindAsync(newTeamMemberDto.ProjectId);
                
                if(user == null || project == null)
                {
                    throw new BadRequestException($"Невозмжоно добавить пользователя в команду проекта, т. к. такого проекта и/или пользователя не существует");
                }

                if (newTeamMemberDto.SupervisorId != null)
                {
                    var supervisor = await _context.TeamMembers.FindAsync(newTeamMemberDto.SupervisorId); 

                    if (supervisor == null)
                    {
                        throw new BadRequestException($"Невозмжоно добавить в команду проекта {project.Name} пользователя {user.Login}, т.к. руководитель, указанный в записи, не существует в системе "); 
                    }
                }

                var existingTeamMember = await _context.TeamMembers.FirstOrDefaultAsync(tm => tm.UserId == newTeamMemberDto.UserId && tm.ProjectId == newTeamMemberDto.ProjectId); 

                if(existingTeamMember != null)
                {
                    throw new BadRequestException($"Невозмжоно добавить в команду проекта {project.Name} пользователя {user.Login}, т.к. этот пользователь уже есть в команде ");
                }

                var newTeamMember = new TeamMember() 
                {
                    UserId = newTeamMemberDto.UserId, 
                    ProjectId = newTeamMemberDto.ProjectId, 
                    Role = newTeamMemberDto.Role, 
                    SupervisorId = newTeamMemberDto.SupervisorId
                };

                await _context.TeamMembers.AddAsync(newTeamMember); 
                await _context.SaveChangesAsync();

                return new TeamMemberDisplayDto(newTeamMember); 
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        // change role 
        public async Task<TeamMemberDisplayDto> UpdateRole(TeamMemberDto teamMemberDto)
        {
            try
            {
                var existingTeamMember = await _context.TeamMembers.FirstOrDefaultAsync(tm => tm.UserId == teamMemberDto.UserId && tm.ProjectId == teamMemberDto.ProjectId);

                if (existingTeamMember == null)
                {
                    throw new BadRequestException($"Невозмжоно изменить роль участника команды, т.к. этого участника команды нет в системе ");
                }

                existingTeamMember.Role = teamMemberDto.Role; 

                await _context.SaveChangesAsync();

                return new TeamMemberDisplayDto(existingTeamMember); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        // change supervisor 
        public async Task<TeamMemberDisplayDto> UpdateSupervisor(TeamMemberDto teamMemberDto)
        {
            try
            {
                var existingTeamMember = await _context.TeamMembers.FirstOrDefaultAsync(tm => tm.UserId == teamMemberDto.UserId && tm.ProjectId == teamMemberDto.ProjectId); 

                if (existingTeamMember == null)
                {
                    throw new BadRequestException($"Невозмжоно изменить руководителя участника команды, т.к. этого участника команды нет в системе ");
                }

                if (teamMemberDto.SupervisorId != null)
                {
                    var supervisor = await _context.TeamMembers.FindAsync(teamMemberDto.SupervisorId);

                    if (supervisor == null)
                    {
                        throw new BadRequestException($"Невозмжоно изменить руководителя участника команды, т.к. этого руководителя нет в системе ");
                    }
                }

                existingTeamMember.SupervisorId = teamMemberDto.SupervisorId; 

                await _context.SaveChangesAsync();

                return new TeamMemberDisplayDto(existingTeamMember);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        // delete by self id
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
