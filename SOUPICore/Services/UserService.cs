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

        public async Task<IEnumerable<UserDto>> Get(CancellationToken ct = default)
        {
            try
            { 
                return await _context.Users.Select(u => new UserDto(u)).ToListAsync(ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<UserDto> GetById(Guid id, CancellationToken ct = default)
        {
            try
            {
                var user = await _context.Users.FindAsync([id], cancellationToken: ct);

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

        public async Task<UserDto> GetByLogin(string login, CancellationToken ct = default)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login, ct);

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

        public async Task<UserDto> Create(UserDto newUserDto, CancellationToken ct = default)
        {
            try
            {
                var user = new User()
                {
                    Login = newUserDto.Login
                };

                await _context.Users.AddAsync(user, ct);
                await _context.SaveChangesAsync(ct);

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
