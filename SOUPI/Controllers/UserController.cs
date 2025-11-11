using Microsoft.AspNetCore.Mvc;
using SOUPIShared.Models;
using SOUPIShared.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace SOUPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger; 
        private readonly SoupiDbContext _context;

        public UserController(ILogger<UserController> logger, SoupiDbContext context)
        {
            _logger = logger; 
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetByLogin([FromQuery] string login)
        {
            try
            {
                var existingUser = await _context.Users.Where(u => u.Login == login).FirstOrDefaultAsync();

                if (existingUser == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(new UserDto(existingUser));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500); 
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] UserDto userDto)
        {
            try
            {
                var existingUser = await _context.Users.Where(u => u.Login == userDto.Login).FirstOrDefaultAsync();

                if (existingUser == null)
                {
                    var user = new User()
                    {
                        Login = userDto.Login
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    return Ok(new UserDto(user));
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                return StatusCode(500); 
            }
        }
    }
} 