using Microsoft.AspNetCore.Mvc;

namespace SOUPIAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase 
    {
        private readonly SoupiDbContext _context;      
        
        public UserController(SoupiDbContext context)
        {
            _context = context;
        }
    }
}
