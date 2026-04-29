using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;

namespace SOUPICore.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class TeamMemberController : ControllerBase
    {
        private readonly ILogger<TeamMemberController> _logger;
        private readonly ITeamMemberService _teamMemberService;

        public TeamMemberController(ILogger<TeamMemberController> logger, ITeamMemberService teamMemberService)
        {
            _logger = logger;
            _teamMemberService = teamMemberService;
        }

        [HttpGet("{teamMemberId}")]
        public async Task<ActionResult<TeamMemberDisplayDto>> GetById([FromRoute] Guid teamMemberId)
        {
            try
            {
                var teamMember = await _teamMemberService.GetById(teamMemberId);

                return Ok(teamMember);
            }
            catch (BadRequestException ex)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<IEnumerable<TeamMemberDisplayDto>>> GetByProjectId([FromRoute] Guid projectId)
        {
            try
            {
                var teamMembers = await _teamMemberService.GetByProjectId(projectId);

                return Ok(teamMembers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TeamMemberDto>> Update([FromBody] TeamMemberDto updatedTeamMemberDto)
        {
            try
            {
                var teamMember = await _teamMemberService.Update(updatedTeamMemberDto);

                return Ok(teamMember);
            }
            catch (BadRequestException ex)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById([FromRoute] Guid id)
        {
            try
            {
                await _teamMemberService.DeleteById(id);

                return Ok();
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }
    }
}
