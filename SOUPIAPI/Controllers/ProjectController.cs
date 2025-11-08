using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOUPIShared.Dtos;
using SOUPIShared.Models;

namespace SOUPIAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProjectController : ControllerBase 
    {
        private readonly SoupiDbContext _context; 

        public ProjectController(SoupiDbContext context)
        {
            _context = context; 
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectsByLogin([FromQuery] string login)
        {
            var existingUser = await _context.Users.Where(u => u.Login == login).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                return BadRequest();
            }

            var projects = await _context.Projects.Where(p => p.CreatorId == existingUser.Id).ToListAsync();
            if (projects == null)
            {
                return BadRequest(); 
            }

            return Ok(projects.Select(p => new ProjectDto(p)));           
        }
    }
} 