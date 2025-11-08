using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Login { get; set; } = null!;

        public UserDto(User user)
        {
            Id = user.Id;
            Login = user.Login; 
        }

        public UserDto() { }
    }
}
