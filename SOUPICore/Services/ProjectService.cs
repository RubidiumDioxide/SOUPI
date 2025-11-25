using Microsoft.Extensions.Logging;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Models; 
using Microsoft.EntityFrameworkCore; 


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
                var projects = await _context.Projects.Where(p => p.CreatorId == userId).ToListAsync(); 

                return projects.Select(p => new ProjectDto(p)); 
            }
            catch (SoupiException ex)
            {
                _logger.LogError($"Не удалось загрузить проекты {ex.Message}");
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить проекты {ex.Message}");
                throw new SoupiException("Не удалось загрузить проекты. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<ProjectDto?> GetById(Guid id)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);

                if (project == null)
                {
                    return null;
                }
                else
                {
                    return new ProjectDto(project);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить редактируемый проект {ex.Message}");
                throw new SoupiException("Не удалось загрузить редактируемый проект. Попробуйте позже или сообщите об ошибке в техподдержку ");
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

                _context.Projects.Add(newProject);
                await _context.SaveChangesAsync();

                return new ProjectDto(newProject);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось создать новый проект. {ex.Message}");
                throw new SoupiException("Не удалось создать новый проект. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<ProjectDto> Update(ProjectDto updatedProjectDto)
        {
            try
            {
                var project = await _context.Projects.FindAsync(updatedProjectDto.Id);

                if (project == null)
                {
                    throw new SoupiException("Проект не найден ");
                }

                project.Name = updatedProjectDto.Name;
                project.Description = updatedProjectDto.Description; 
                project.Image = updatedProjectDto.Image; 

                await _context.SaveChangesAsync();

                return new ProjectDto(project);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось сохранить изменения: {ex.Message}");
                throw new SoupiException("Не удалось сохранить изменения ");
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);

                if (project == null)
                {
                    throw new SoupiException("Проект не найден ");
                }

                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить проект: {ex.Message}");
                throw new SoupiException("Не удалось удалить проект ");
            }
        }
    }
}
