using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;


namespace SOUPICore.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class AssignmentController : ControllerBase 
    {
        private readonly ILogger<AssignmentController> _logger;
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(ILogger<AssignmentController> logger, IAssignmentService assignmentService)
        {
            _logger = logger;
            _assignmentService = assignmentService;
        }

        [HttpGet("{assignmentId}")]
        public async Task<ActionResult<AssignmentDisplayDto>> GetById([FromRoute] Guid assignmentId)
        {
            try
            {
                var assignment = await _assignmentService.GetById(assignmentId);

                return Ok(assignment);
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
                var assignments = await _assignmentService.GetByProjectId(projectId);

                return Ok(assignments);
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
                var assignments = await _assignmentService.GetByJobId(jobId); 

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<AssignmentDisplayDto>>> GetByUserId([FromRoute] Guid userId)
        {
            try
            {
                var assignments = await _assignmentService.GetByUserId(userId);

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<AssignmentDto>> Create([FromBody] AssignmentDto newAssignmentDto)
        {
            try
            {
                var assignment = await _assignmentService.Create(newAssignmentDto);

                return Ok(assignment);
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
        public async Task<ActionResult<AssignmentDto>> UpdateContent([FromBody] AssignmentDto updatedAssignmentDto)
        {
            try
            {
                var assignment = await _assignmentService.UpdateContent(updatedAssignmentDto);

                return Ok(assignment); 
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

        [HttpGet("{assignmentId}")]
        public async Task<ActionResult> Delete([FromRoute] Guid assignmentId)
        {
            try
            {
                await _assignmentService.Delete(assignmentId);

                return Ok();
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
    }
}
