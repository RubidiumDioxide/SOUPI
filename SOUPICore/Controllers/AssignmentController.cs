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
    public class AssignmentController : ControllerBase 
    {
        private readonly ILogger<AssignmentController> _logger;
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(ILogger<AssignmentController> logger, IAssignmentService assignmentService)
        {
            _logger = logger;
            _assignmentService = assignmentService;
        }

        [HttpGet("{jobId}")]
        public async Task<ActionResult<JobDto>> GetById([FromRoute] Guid jobId)
        {
            try
            {
                var assignment = await _assignmentService.GetByJobId(jobId);

                return Ok(assignment);
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
