using SOUPI.Handlers.Interfaces;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;


namespace SOUPI.Handlers
{
    public class TeamMemberRequestHandler : ITeamMemberRequestHandler 
    {
        private readonly ILogger<TeamMemberRequestHandler> _logger;
        private readonly ITeamMemberService _teamMemberService;

        public TeamMemberRequestHandler(ILogger<TeamMemberRequestHandler> logger, ITeamMemberService teamMemberService)
        {
            _logger = logger;
            _teamMemberService = teamMemberService;
        }

        public async Task<TeamMemberDisplayDto> GetById(Guid teamMemberId, CancellationToken ct = default)
        {
            try
            {
                return await _teamMemberService.GetById(teamMemberId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить участника команды проекта: {ex.Message}");
                throw new SoupiException("Не удалось загрузить участника команды проекта. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<TeamMemberDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                return await _teamMemberService.GetByProjectId(projectId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось загрузить участников команды проекта: {ex.Message}");
                throw new SoupiException("Не удалось загрузить участников команды проекта. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<TeamMemberDto> Update(TeamMemberDto updatedTeamMemberDto, CancellationToken ct = default)
        {
            try
            {
                return await _teamMemberService.Update(updatedTeamMemberDto, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось сохранить изменения: {ex.Message}");
                throw new SoupiException("Не удалось сохранить изменения. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task Delete(Guid id, CancellationToken ct = default)
        {
            try
            {
                await _teamMemberService.Delete(id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось исключить участника из команды проекта: {ex.Message}");
                throw new SoupiException("Не удалось исключить участника из команды проекта. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }
    }
}
