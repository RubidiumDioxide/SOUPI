using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPICore.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDto>> Get(); 

        public Task<UserDto> GetById(Guid id); 
        
        public Task<UserDto> GetByLogin(string login);

        public Task<UserDto> Create(UserDto newUserDto);
    }
}
