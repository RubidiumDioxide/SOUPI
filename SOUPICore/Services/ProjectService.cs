using Microsoft.Extensions.Logging;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Models; 
using Microsoft.EntityFrameworkCore;
using SOUPICore.Services.Interfaces;


namespace SOUPICore.Services
{
    public class ProjectService : IProjectService 
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly SoupiDbContext _context;

        public ProjectService(ILogger<ProjectService> logger, SoupiDbContext context)
        {
            _logger = logger;
            _context = context;  
        } 

        public async Task<IEnumerable<ProjectDto>> GetByUserId(Guid userId)
        {
            try
            {
                var projects = await _context.Projects
                    .Where(p => p.TeamMembers
                    .Select(tm => tm.UserId)
                    .Contains(userId))
                    .ToListAsync(); 

                return projects.Select(p => new ProjectDto(p)); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw; 
            }
        }

        public async Task<ProjectDto> GetById(Guid id)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);

                if (project == null)
                {
                    throw new NotFoundException(); 
                }
                else
                {
                    return new ProjectDto(project);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw; 
            }
        }

        public async Task<ProjectDto> Create(ProjectDto newProjectDto)
        {
            try
            {
                var newProject = new Project()
                {
                    Name = newProjectDto.Name,
                    Description = newProjectDto.Description,
                    GithubRepository = newProjectDto.GithubRepository,
                    CreatorId = newProjectDto.CreatorId, 
                    StartDateTime = newProjectDto.StartDateTime, 
                };

                await _context.Projects.AddAsync(newProject);

                var newTeamMember = new TeamMember()
                {
                    UserId = newProject.CreatorId,
                    ProjectId = newProject.Id,
                    Role = "Основатель проекта", 
                    SupervisorId = null
                };

                await _context.AddAsync(newTeamMember); 
                await _context.SaveChangesAsync();

                return new ProjectDto(newProject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); 
                throw; 
            }
        }

        public async Task<ProjectDto> Update(ProjectDto updatedProjectDto)
        {
            try
            {
                var project = await _context.Projects.FindAsync(updatedProjectDto.Id);

                if (project == null)
                {
                    throw new NotFoundException(); 
                }

                project.Name = updatedProjectDto.Name;
                project.Description = updatedProjectDto.Description; 

                await _context.SaveChangesAsync();

                return new ProjectDto(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); 
                throw;    
            }
        }

        public async Task<ProjectDto> ChangeCreator(ProjectDto updatedProjectDto)
        {
            try
            {
                var project = await _context.Projects.FindAsync(updatedProjectDto.Id);

                if (project == null)
                {
                    throw new BadRequestException("Руководителя проекта нельзя переназначить, т.к. проект не найден в системе ");
                }

                var previousCreatorTeamMember = await _context.TeamMembers
                    .FirstOrDefaultAsync(tm => tm.ProjectId == project.Id && tm.UserId == project.CreatorId);
                var newCreatorTeamMember = await _context.TeamMembers
                    .FirstOrDefaultAsync(tm => tm.ProjectId == project.Id && tm.UserId == updatedProjectDto.CreatorId);

                if (previousCreatorTeamMember == null || newCreatorTeamMember == null)
                {
                    throw new BadRequestException("Руководителя проекта нельзя переназначить, т.к. соответствующие записи участников команды не найдены в системе ");
                }

                // link all newCreator's subservient to it's supervisor 
                newCreatorTeamMember.Subservient.Select(tm => tm.SupervisorId = newCreatorTeamMember.SupervisorId);
                // link all oldCreator's subservient to mewCreator 
                previousCreatorTeamMember.Subservient.Where(tm => tm.Id != newCreatorTeamMember.Id).Select(tm => tm.SupervisorId = newCreatorTeamMember.Id); 
                newCreatorTeamMember.SupervisorId = null; 
                project.CreatorId = newCreatorTeamMember.UserId; 

                await _context.SaveChangesAsync();

                return new ProjectDto(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);

                if (project == null)
                {
                    throw new BadRequestException("Проект нельзя удалить, т.к. он не найден в системе ");  
                }

                var teamMembers = project.TeamMembers;
                var notifications = project.Notifications; 
                var jobs = project.Jobs;

                _context.TeamMembers.RemoveRange(teamMembers); 
                _context.Notifications.RemoveRange(notifications); 
                _context.Jobs.RemoveRange(jobs); 
                _context.Projects.Remove(project); 
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); 
                throw;
            }
        }
    }
}
