using SOUPIShared.Models;
using System.ComponentModel.DataAnnotations;
using SOUPIShared.Attributes;


namespace SOUPIShared.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }

        [Required]
        [ValidGitHubUsername]
        public string Login { get; set; } = null!;
    

        public UserDto(User user)
        {
            Id = user.Id; 
            Login = user.Login; 
        }

        public UserDto() { }
    }
}
