using SOUPIShared.Exceptions;
using SOUPI.Handlers.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPICore.Services.Interfaces;


namespace SOUPI.Handlers
{
    public class UserRequestHandler : IUserRequestHandler
    {
        private readonly ILogger<UserRequestHandler> _logger;
        private readonly IUserService _userService;

        public UserRequestHandler(ILogger<UserRequestHandler> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<UserDto> Create(UserDto userDto, CancellationToken ct = default)
        {
            try
            {
                return await _userService.Create(userDto, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось зарегистрировать нового пользователя. {ex.Message}");
                throw new SoupiException("Не удалось зарегистрировать нового пользователя. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<IEnumerable<UserDto>> Get(CancellationToken ct = default)
        {
            try
            {
                return await _userService.Get(ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<UserDto?> GetById(Guid id, CancellationToken ct = default)
        {
            try
            {
                return await _userService.GetById(id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            }
        }

        public async Task<UserDto?> GetByLogin(string login, CancellationToken ct = default)
        {
            try
            {
                return await _userService.GetByLogin(login, ct); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить информацию о пользовтеле. {ex.Message}");
                throw new SoupiException("Не удалось получить информацию о пользовтеле. Попробуйте позже или сообщите об ошибке в техподдержку ");
            } 
        }
    }
}
