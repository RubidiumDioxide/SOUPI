using SOUPIShared.Dtos; 


namespace SOUPI.Services
{
    public interface IUserService
    {
        public Task<UserDto> SaveNewUser(UserDto userDto); 

        public Task<UserDto?> GetUserByLogin(string login); 
    }
}
