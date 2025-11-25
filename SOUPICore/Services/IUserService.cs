using SOUPIShared.Dtos; 


namespace SOUPICore.Services
{
    public interface IUserService
    {
        public Task<UserDto> Create(UserDto newUserDto); 

        public Task<UserDto?> GetByLogin(string login); 
    }
}
