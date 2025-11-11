using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOUPIShared.Dtos;
using SOUPIShared.Models;
using Microsoft.AspNetCore.JsonPatch;


namespace SOUPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ProjectController : ControllerBase 
    {
        private readonly ILogger<ProjectController> _logger; 
        private readonly SoupiDbContext _context; 

        public ProjectController(ILogger<ProjectController> logger, SoupiDbContext context)
        {
            _logger = logger; 
            _context = context; 
        }

        [HttpGet("{login}")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByLogin([FromRoute] string login)
        {
            try
            {
                var existingUser = await _context.Users.Where(u => u.Login == login).FirstOrDefaultAsync();
                if (existingUser == null)
                {
                    return BadRequest();
                }

                var projects = await _context.Projects.Where(p => p.CreatorId == existingUser.Id).ToListAsync();

                return Ok(projects.Select(p => new ProjectDto(p)));
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
                var newProject = new Project()
                {
                    Name = projectDto.Name,
                    Description = projectDto.Description,
                    GithubRepository = projectDto.GithubRepository,
                    CreatorId = projectDto.CreatorId,
                    Image = projectDto.Image,
                };

                _context.Projects.Add(newProject);
                await _context.SaveChangesAsync();

                return Ok(new ProjectDto(newProject));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); 
                return StatusCode(500); 
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ProjectDto>> Update([FromRoute] Guid id, [FromBody] JsonPatchDocument<Project> patchDoc)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);

                if (project == null)
                {
                    return BadRequest();
                }

                patchDoc.ApplyTo(project);

                _context.SaveChanges();
                return Ok(new ProjectDto(project));
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
                var project = await _context.Projects.FindAsync(id);

                if (project == null)
                {
                    return BadRequest();
                }

                _context.Projects.Remove(project);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }
    }
} 