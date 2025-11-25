using SOUPIShared.Exceptions;
using SOUPIShared.Dtos;
using Microsoft.Extensions.Logging; 
using Microsoft.EntityFrameworkCore;
using SOUPIShared.Models; 


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
                _logger.LogError($"Не удалось зарегистрировать нового польхователя. {ex.Message}");
                throw new SoupiException("Не удалось зарегистрировать нового польхователя. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        } 

        public async Task<UserDto?> GetByLogin(string login)
        {
            try
            {
                var user = await _context.Users.Where(u => u.Login == login).FirstOrDefaultAsync();

                if (user == null)
                {
                    return null; 
                }
                else
                {
                    return new UserDto(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о текущем пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о текущем пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            } 
        }
    }
}
