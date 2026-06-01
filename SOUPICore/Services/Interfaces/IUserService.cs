using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPICore.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDto>> Get(CancellationToken ct = default); 

        public Task<UserDto> GetById(Guid id, CancellationToken ct = default); 

        public Task<UserDto?> GetByLogin(string login, CancellationToken ct = default);

        public Task<UserDto> Create(UserDto newUserDto, CancellationToken ct = default);
    }
}
