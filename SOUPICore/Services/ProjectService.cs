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
                    Image = newProjectDto.Image,
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
                project.Image = updatedProjectDto.Image; 

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
