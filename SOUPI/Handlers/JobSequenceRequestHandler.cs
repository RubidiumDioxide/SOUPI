using SOUPI.Handlers.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;
using System.Text.Json;
using SOUPICore.Services.Interfaces; 


namespace SOUPI.Handlers
{
    public class JobSequenceRequestHandler : IJobSequenceRequestHandler
    {
        private readonly ILogger<JobSequenceRequestHandler> _logger;
        private readonly IJobSequenceService _jobSequenceService; 

        public JobSequenceRequestHandler(ILogger<JobSequenceRequestHandler> logger, IJobSequenceService jobSequenceService)
        {
            _logger = logger;
            _jobSequenceService = jobSequenceService;
        }

        public async Task<IEnumerable<JobSequenceDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                return await _jobSequenceService.GetByProjectId(projectId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить связи между задачами {ex.Message}");
                throw new SoupiException("Не удалось загрузить связи между задачами. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }
        
        public async Task<JobSequenceDto> Create(Guid firstJobId, Guid secondJobId, CancellationToken ct = default)
        {
            try
            {
               return await _jobSequenceService.Create(firstJobId, secondJobId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось создать новую связь между задачами. {ex.Message}");
                throw new SoupiException("Не удалось создать новую связь между задачами. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task Delete(Guid jobSequenceId, CancellationToken ct = default)
        {
            try
            {
                await _jobSequenceService.Delete(jobSequenceId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить связь между задачами: {ex.Message}");
                throw new SoupiException("Не удалось удалить связь между задачами ");
            }
        }
    }
}
