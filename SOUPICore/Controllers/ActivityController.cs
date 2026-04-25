using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;


namespace SOUPICore.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ActivityController : ControllerBase 
    {
        private readonly ILogger<ActivityController> _logger;
        private readonly IActivityService _activityService;

        public ActivityController(ILogger<ActivityController> logger, IActivityService activityService)
        {
            _logger = logger;
            _activityService = activityService;
        }

        [HttpGet("{assignmentId}")]
        public async Task<ActionResult<IEnumerable<AssignmentDisplayDto>>> GetByAssignmentId([FromRoute] Guid assignmentId)
        {
            try
            {
                var activities = await _activityService.GetByAssignmentId(assignmentId);

                return Ok(activities);
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

        [HttpGet("{teamMemberId}")]
        public async Task<ActionResult<IEnumerable<AssignmentDisplayDto>>> GetByTeamMemberId([FromRoute] Guid teamMemberId)
        {
            try
            {
                var activities = await _activityService.GetByTeamMemberId(teamMemberId);

                return Ok(activities);
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


        [HttpGet("{jobId}")]
        public async Task<ActionResult<IEnumerable<AssignmentDisplayDto>>> GetByJobId([FromRoute] Guid jobId)
        {
            try
            {
                var activities = await _activityService.GetByJobId(jobId);

                return Ok(activities); 
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

        [HttpGet("{projectId}")]
        public async Task<ActionResult<IEnumerable<AssignmentDisplayDto>>> GetByProjectId([FromRoute] Guid projectId)
        {
            try
            {
                var activities = await _activityService.GetByProjectId(projectId);

                return Ok(activities);
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

        [HttpPost]
        public async Task<ActionResult<ActivityDto>> Create([FromBody] ActivityDto newActivityDto)
        {
            try
            {
                var activity = await _activityService.Create(newActivityDto); 

                return Ok(activity);
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

        [HttpPost]
        public async Task<ActionResult<ActivityDto>> UpdateContent([FromBody] ActivityDto updatedActivityDto)
        {
            try
            {
                var activity = await _activityService.UpdateContent(updatedActivityDto);

                return Ok(activity); 
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

        [HttpGet("{activityId}")]
        public async Task<ActionResult> Delete([FromRoute] Guid activityId)
        {
            try
            {
                await _activityService.Delete(activityId);

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
