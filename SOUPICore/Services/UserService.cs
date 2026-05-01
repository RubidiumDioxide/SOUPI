using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SOUPIShared.Models;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Exceptions;
using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPICore.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly SoupiDbContext _context; 

        public UserService(ILogger<UserService> logger, SoupiDbContext context)
        {
            _logger = logger;
            _context = context; 
        }

        public async Task<IEnumerable<UserDto>> Get()
        {
            try
            { 
                return await _context.Users.Select(u => new UserDto(u)).ToListAsync(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<UserDto> GetById(Guid id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    throw new NotFoundException();
                }
                else
                {
                    return new UserDto(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        } 

        public async Task<UserDto> GetByLogin(string login)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);

                if (user == null)
                {
                    throw new NotFoundException(); 
                }
                else
                {
                    return new UserDto(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); 
                throw; 
            } 
        }

        public async Task<UserDto> Create(UserDto newUserDto)
        {
            try
            {
                var user = new User()
                {
                    Login = newUserDto.Login
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new UserDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
