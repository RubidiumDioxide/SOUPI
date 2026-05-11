using SOUPI.Handlers.Interfaces;
using SOUPIShared.Exceptions;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPICore.Services.Interfaces;


namespace SOUPI.Handlers
{
    public class ProjectRequestHandler : IProjectRequestHandler 
    {
        private readonly ILogger<ProjectRequestHandler> _logger;
        private readonly IProjectService _projectService;

        public ProjectRequestHandler(ILogger<ProjectRequestHandler> logger, IProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }

        public async Task<IEnumerable<ProjectDisplayDto>> GetByUserId(Guid userId, CancellationToken ct = default)
        {
            try
            {
                return await _projectService.GetByUserId(userId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить проекты {ex.Message}");
                throw new SoupiException("Не удалось загрузить проекты. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<ProjectDisplayDto?> GetById(Guid id, CancellationToken ct = default)
        {
            try
            {
                return await _projectService.GetById(id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить проект {ex.Message}");
                throw new SoupiException("Не удалось загрузить проект. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<ProjectDto> Create(ProjectDto projectDto, CancellationToken ct = default)
        {
            try
            {
                return await _projectService.Create(projectDto, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось создать новый проект. {ex.Message}");
                throw new SoupiException("Не удалось создать новый проект. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<ProjectDto> Update(ProjectDto updatedProjectDto, CancellationToken ct = default)
        {
            try
            {
                return await _projectService.Update(updatedProjectDto, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось сохранить изменения: {ex.Message}");
                throw new SoupiException("Не удалось сохранить изменения ");
            }
        }

        public async Task<ProjectDto> SetGitHubRepository(Guid projectId, string repositoryName, CancellationToken ct = default)
        {
            try
            {
                return await _projectService.SetGitHubRepository(projectId, repositoryName, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось привязать репозиторий к проекту: {ex.Message}");
                throw new SoupiException("Не удалось привязать репозиторий к проекту ");
            }
        }

        public async Task Delete(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                await _projectService.Delete(projectId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить проект: {ex.Message}");
                throw new SoupiException("Не удалось удалить проект ");
            }
        }
    }
}
