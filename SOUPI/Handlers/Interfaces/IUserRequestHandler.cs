using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPI.Handlers.Interfaces
{
    public interface IUserRequestHandler 
    {
        public Task<UserDto> Create(UserDto userDto, CancellationToken ct = default);

        public Task<IEnumerable<UserDto>> Get(CancellationToken ct = default);

        public Task<UserDto?> GetById(Guid id, CancellationToken ct = default); 
        
        public Task<UserDto?> GetByLogin(string login, CancellationToken ct = default); 
    }
}
