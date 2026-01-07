using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOUPIShared.Dtos;
using Microsoft.Extensions.Logging;
using SOUPIShared.Exceptions;
using SOUPICore.Services.Interfaces;


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
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByUserId([FromRoute] Guid userId)
        {
            try
            {
                var projects = await _projectService.GetByCreatorId(userId); 

                return Ok(projects);
            }
            catch (SoupiException ex)
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
        public async Task<ActionResult<ProjectDto?>> GetById([FromRoute] Guid id)
        {
            try
            {
                var project = await _projectService.GetById(id); 

                if (project == null)
                {
                    return NotFound();   
                }
                else
                {
                    return Ok(project);
                }
            }
            catch (SoupiException ex)
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
            catch (SoupiException ex)
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
            catch (SoupiException ex)
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
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                await _projectService.Delete(id);   

                return Ok();
            }
            catch (SoupiException ex)
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