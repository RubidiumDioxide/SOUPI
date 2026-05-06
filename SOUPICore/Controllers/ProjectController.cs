using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SOUPIShared.Exceptions;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPICore.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ProjectController : ControllerBase 
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IProjectService _projectService; 

        public ProjectController(ILogger<ProjectController> logger, IProjectService projectService)
        {
            _logger = logger; 
            _projectService = projectService; 
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<ProjectDisplayDto>>> GetByUserId([FromRoute] Guid userId)
        {
            try
            {
                var projects = await _projectService.GetByUserId(userId); 

                return Ok(projects);
            }
            catch (NotFoundException)
            {
                return NotFound();
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDisplayDto?>> GetById([FromRoute] Guid id)
        {
            try
            {
                var project = await _projectService.GetById(id); 

                return Ok(project);               
            }
            catch (NotFoundException)
            {
                return NotFound();
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
        public async Task<ActionResult<ProjectDto>> Create([FromBody] ProjectDto projectDto)
        {
            try
            {
                var project = await _projectService.Create(projectDto); 

                return Ok(project);
            }
            catch (NotFoundException)
            {
                return NotFound();
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
        public async Task<ActionResult<ProjectDto>> Update([FromBody] ProjectDto updatedProjectDto)
        {
            try
            {
                var project = await _projectService.Update(updatedProjectDto); 

                return Ok(project);
            }
            catch (NotFoundException)
            {
                return NotFound(); 
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
        public async Task<ActionResult<ProjectDto>> UpdateCreator([FromBody] ProjectDto updatedProjectDto)
        {
            try
            {
                var project = await _projectService.UpdateCreator(updatedProjectDto); 

                return Ok(project);
            }
            catch (NotFoundException)
            {
                return NotFound();
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

        [HttpGet("{projectId}/{repositoryName}")]
        public async Task<ActionResult<ProjectDto>> SetGitHubRepository([FromRoute] Guid projectId, [FromRoute] string repositoryName)
        {
            try
            {
                var project = await _projectService.SetGitHubRepository(projectId, repositoryName);

                return Ok(project);
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

        [HttpGet("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                await _projectService.Delete(id);   

                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
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