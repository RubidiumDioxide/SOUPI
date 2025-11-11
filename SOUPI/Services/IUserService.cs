using SOUPIShared.Dtos; 


namespace SOUPI.Services
{
    public interface IUserService
    {
        public Task<UserDto> Create(UserDto userDto); 

        public Task<UserDto?> GetByLogin(string login); 
    }
}
