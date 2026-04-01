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
    public class JobSequenceController : ControllerBase
    {
        private readonly ILogger<JobSequenceController> _logger;
        private readonly IJobSequenceService _jobSequenceService;

        public JobSequenceController(ILogger<JobSequenceController> logger, IJobSequenceService jobSequenceService)
        {
            _logger = logger;
            _jobSequenceService = jobSequenceService;
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<IEnumerable<JobSequenceDisplayDto>>> GetByProjectId([FromRoute] Guid projectId)
        {
            try
            {
                var jobSequences = await _jobSequenceService.GetByProjectId(projectId); 

                return Ok(jobSequences);
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

        [HttpGet("{firstJobId}/{secondJobId}")]
        public async Task<ActionResult<JobSequenceDto>> Create([FromRoute] Guid firstJobId, [FromRoute] Guid secondJobId)
        {
            try
            {
                var jobSequence = await _jobSequenceService.Create(firstJobId, secondJobId); 

                return Ok(jobSequence);
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

        [HttpGet("{jobSequenceId}")]
        public async Task<ActionResult> Delete([FromRoute] Guid jobSequenceId)
        {
            try
            {
                await _jobSequenceService.Delete(jobSequenceId);

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
