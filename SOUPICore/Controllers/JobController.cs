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
    public class JobController : ControllerBase 
    {
        private readonly ILogger<JobController> _logger;
        private readonly IJobService _jobService;

        public JobController(ILogger<JobController> logger, IJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }


        [HttpGet("{jobId}")]
        public async Task<ActionResult<JobDto>> GetById([FromRoute] Guid jobId)
        {
            try
            {
                var job = await _jobService.GetById(jobId);

                return Ok(job); 
            }
            catch (NotFoundException)
            {
                return NotFound(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }


        [HttpGet("{projectId}")]
        public async Task<ActionResult<IEnumerable<JobDto>>> GetByProjectId([FromRoute] Guid projectId)
        {
            try
            {
                var jobs = await _jobService.GetByProjectId(projectId);    

                return Ok(jobs); 
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
        public async Task<ActionResult<JobDto>> Create([FromBody] JobDto newJobDto)
        {
            try
            {
                var job = await _jobService.Create(newJobDto);

                return Ok(job);
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
        public async Task<ActionResult<JobDto>> UpdateContent([FromBody] JobDto updatedJobDto)
        {
            try
            {
                var job = await _jobService.UpdateContent(updatedJobDto);

                return Ok(job);
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

        [HttpPost("{jobId}/{newParentId}")]
        public async Task<ActionResult<JobDto>> UpdateParent([FromRoute] Guid jobId, [FromRoute] Guid? newParentId)
        {
            try
            {
                var job = await _jobService.UpdateParent(jobId, newParentId);

                return Ok(job);
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

        [HttpGet("{jobId}/{preserveChildren}")]
        public async Task<ActionResult> Delete([FromRoute] Guid jobId, [FromRoute] bool preserveChildren)
        {
            try
            {
                await _jobService.Delete(jobId, preserveChildren);

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
