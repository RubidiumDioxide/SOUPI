using Microsoft.AspNetCore.Mvc;
using SOUPIShared.Models;
using SOUPIShared.Dtos;
using Microsoft.EntityFrameworkCore;


namespace SOUPIAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly SoupiDbContext _context;

        public UserController(SoupiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUserByLogin([FromQuery] string login)
        {
            var existingUser = await _context.Users.Where(u => u.Login == login).FirstOrDefaultAsync();

            if(existingUser == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(new UserDto(existingUser)); 
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> SaveNewUser([FromBody] UserDto userDto)
        {
            var existingUser = await _context.Users.Where(u => u.Login == userDto.Login).FirstOrDefaultAsync(); 

            if (existingUser == null)
            {
                var user = new User() { 
                    Login = userDto.Login
                };

                _context.Users.Add(user);  
                _context.SaveChanges();

                return Ok(new UserDto(user));
            }
            else
            {
                return BadRequest(); 
            }
        }
    }
} 