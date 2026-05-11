using SOUPI.Handlers.Interfaces;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;


namespace SOUPI.Handlers
{
    public class ActivityRequestHandler : IActivityRequestHandler
    {
        private readonly ILogger<ActivityRequestHandler> _logger;
        private readonly IActivityService _activityService; 

        public ActivityRequestHandler(ILogger<ActivityRequestHandler> logger, IActivityService activityService)
        {
            _logger = logger;
            _activityService = activityService;
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByAssignmentId(Guid assignmentId, CancellationToken ct = default)
        {
            try
            {
                return await _activityService.GetByAssignmentId(assignmentId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить действия {ex.Message}");
                throw new SoupiException("Не удалось загрузить действия. Попробуйте позже или сообщите об ошибке в техподдержку "); 
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByTeamMemberId(Guid teamMemberId, CancellationToken ct = default)
        {
            try
            {
                return await _activityService.GetByTeamMemberId(teamMemberId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить действия {ex.Message}");
                throw new SoupiException("Не удалось загрузить действия. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByJobId(Guid jobId, CancellationToken ct = default)
        {
            try
            {
                return await _activityService.GetByJobId(jobId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить действия {ex.Message}");
                throw new SoupiException("Не удалось загрузить действия. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                return await _activityService.GetByProjectId(projectId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить действия {ex.Message}");
                throw new SoupiException("Не удалось загрузить действия. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<ActivityDto> Create(ActivityDto activityDto, CancellationToken ct = default)
        {
            try
            {
                return await _activityService.Create(activityDto, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось добавить действие {ex.Message}");
                throw new SoupiException("Не удалось добавить действие. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<ActivityDto> UpdateContent(ActivityDto updatedActivityDto, CancellationToken ct = default)
        {
            try
            {
                return await _activityService.UpdateContent(updatedActivityDto, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось изменить действие {ex.Message}");
                throw new SoupiException("Не удалось изменить действие. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task Delete(Guid activityId, CancellationToken ct = default)
        {
            try
            {
                await _activityService.Delete(activityId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить действие {ex.Message}");
                throw new SoupiException("Не удалось удалить действие ");
            }
        }
    }
}
