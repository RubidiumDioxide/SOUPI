using SOUPIShared.Dtos;

namespace SOUPI.Handlers.Interfaces
{
    public interface IUserRequestHandler 
    {
        public Task<UserDto> Create(UserDto userDto);

        public Task<UserDto?> GetById(Guid id); 
        
        public Task<UserDto?> GetByLogin(string login); 
    }
}
