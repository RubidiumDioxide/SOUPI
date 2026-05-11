using SOUPI.Handlers.Interfaces;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;


namespace SOUPI.Handlers
{
    public class AssignmentRequestHandler : IAssignmentRequestHandler
    {
        private readonly ILogger<AssignmentRequestHandler> _logger;
        private readonly IAssignmentService _assignmentService; 

        public AssignmentRequestHandler(ILogger<AssignmentRequestHandler> logger, IAssignmentService assignmentService)
        {
            _logger = logger;
            _assignmentService = assignmentService; 
        }

        public async Task<AssignmentDisplayDto> GetById(Guid assignmentId, CancellationToken ct = default)
        {
            try
            {
                return await _assignmentService.GetById(assignmentId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить исполнителя{ex.Message}");
                throw new SoupiException("Не удалось загрузить исполнителя. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }
        
        public async Task<IEnumerable<AssignmentDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                return await _assignmentService.GetByProjectId(projectId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить исполнителей {ex.Message}");
                throw new SoupiException("Не удалось загрузить исполнителей. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<AssignmentDisplayDto>> GetByJobId(Guid jobId, CancellationToken ct = default)
        {
            try
            {
                return await _assignmentService.GetByJobId(jobId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить исполнителей {ex.Message}");
                throw new SoupiException("Не удалось загрузить исполнителей. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<AssignmentDisplayDto>> GetByUserId(Guid userId, CancellationToken ct = default)
        {
            try
            {
               return await _assignmentService.GetByUserId(userId, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить исполнителей {ex.Message}");
                throw new SoupiException("Не удалось загрузить исполнителей. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<AssignmentDto> Create(AssignmentDto assignmentDto, CancellationToken ct = default)
        {
            try
            {
                return await _assignmentService.Create(assignmentDto, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось добавить исполнителя {ex.Message}");
                throw new SoupiException("Не удалось добавить исполнителя. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<AssignmentDto> UpdateContent(AssignmentDto updatedAssignmentDto, CancellationToken ct = default)
        {
            try
            {
                return await _assignmentService.UpdateContent(updatedAssignmentDto, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось изменить назначение {ex.Message}");
                throw new SoupiException("Не удалось изменить назначение. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task Delete(Guid assignmentId, CancellationToken ct = default)
        {
            try
            {
                await _assignmentService.Delete(assignmentId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось удалить исполнителя {ex.Message}");
                throw new SoupiException("Не удалось удалить исполнителя ");
            }
        }
    }
}
