using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace SOUPICore.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        
        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger; 
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            try
            {
                return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Logout()
        {
            var user = User;

            return SignOut(new AuthenticationProperties{ RedirectUri = "/" }); 
        }
    }
}