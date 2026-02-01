using SOUPIShared.Dtos;


namespace SOUPICore.Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserDto> Create(UserDto newUserDto); 

        public Task<UserDto> GetById(Guid id); 
        
        public Task<UserDto> GetByLogin(string login); 
    }
}
