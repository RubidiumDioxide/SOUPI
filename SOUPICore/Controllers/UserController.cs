using Microsoft.AspNetCore.Mvc;
using SOUPIShared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SOUPIShared.Exceptions;
using SOUPICore.Services.Interfaces;


namespace SOUPICore.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService; 

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger; 
            _userService = userService;  
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get()
        {
            try
            {
                var user = await _userService.Get(); 

                return Ok(user);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (SoupiException)
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
        public async Task<ActionResult<UserDto?>> GetById([FromRoute] Guid id)
        {
            try
            {
                var user = await _userService.GetById(id);

                return Ok(user);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (SoupiException)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{login}")]
        public async Task<ActionResult<UserDto?>> GetByLogin([FromRoute] string login)
        {
            try
            {
                var user = await _userService.GetByLogin(login);

                return Ok(user);
            }
            catch (NotFoundException)
            {
                return NotFound(); 
            }
            catch (SoupiException)
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
        public async Task<ActionResult<UserDto>> Create([FromBody] UserDto newUserDto)
        {
            try
            {
                var newUser = await _userService.Create(newUserDto); 
                
                return Ok(newUser);
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