using SOUPI.Handlers.Interfaces;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;


namespace SOUPI.Handlers
{
    public class JobRequestHandler : IJobRequestHandler
    {
        private readonly ILogger<JobRequestHandler> _logger;
        private readonly IJobService _jobService; 

        public JobRequestHandler(ILogger<JobRequestHandler> logger, IJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }

        public async Task<JobDisplayDto> GetDisplayById(Guid jobId, CancellationToken ct = default)
        {
            try
            {
                return await _jobService.GetDisplayById(jobId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить задачу {ex.Message}");
                throw new SoupiException("Не удалось загрузить задачу. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                return await _jobService.GetDisplayByProjectId(projectId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить задачи {ex.Message}");
                throw new SoupiException("Не удалось загрузить задачи. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectIdParentId(Guid projectId, Guid? parentJobId, CancellationToken ct = default)
        {
            try
            {
                return await _jobService.GetDisplayByProjectIdParentId(projectId, parentJobId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить задачи {ex.Message}");
                throw new SoupiException("Не удалось загрузить задачи. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<JobDisplayDto>> GetDisplayByUserId(Guid userId, CancellationToken ct = default)
        {
            try
            {
                return await _jobService.GetDisplayByUserId(userId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить задачи {ex.Message}");
                throw new SoupiException("Не удалось загрузить задачи. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<JobDto> GetById(Guid jobId, CancellationToken ct = default)
        {
            try
            {
                return await _jobService.GetById(jobId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить задачу {ex.Message}");
                throw new SoupiException("Не удалось загрузить задачу. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<JobDto>> GetByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                return await _jobService.GetByProjectId(projectId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить задачи {ex.Message}");
                throw new SoupiException("Не удалось загрузить задачи. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<JobDto>> GetByProjectIdParentId(Guid projectId, Guid? parentJobId, CancellationToken ct = default)
        {
            try
            {
                return await _jobService.GetByProjectIdParentId(projectId, parentJobId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить задачи {ex.Message}");
                throw new SoupiException("Не удалось загрузить задачи. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<JobDto> Create(JobDto jobDto, CancellationToken ct = default)
        {
            try
            {
                return await _jobService.Create(jobDto, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось создать новую задачу. {ex.Message}");
                throw new SoupiException("Не удалось создать новую задачу. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<JobDto> UpdateContent(JobDto updatedJobDto, CancellationToken ct = default)
        {
            try
            {
                return await _jobService.UpdateContent(updatedJobDto, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось сохранить изменения: {ex.Message}");
                throw new SoupiException("Не удалось сохранить изменения ");
            }
        }

        public async Task<JobDto> UpdateParent(Guid jobId, Guid? newParentJobId, CancellationToken ct = default)
        {
            try
            {
                return await _jobService.UpdateParent(jobId, newParentJobId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось сохранить изменения: {ex.Message}");
                throw new SoupiException("Не удалось сохранить изменения ");
            }
        }

        public async Task Delete(Guid jobId, bool preserveChildren, CancellationToken ct = default)
        {
            try
            {
                await _jobService.Delete(jobId, preserveChildren, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить задачу: {ex.Message}");
                throw new SoupiException("Не удалось удалить задачу ");
            }
        }
    }
}
