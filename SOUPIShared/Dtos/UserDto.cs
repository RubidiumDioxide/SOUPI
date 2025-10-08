using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? GithubUser { get; set; }

        public string? Image { get; set; }

        public UserDto(User user)
        {
            Id = user.Id;
            Name = user.Name;
            GithubUser = user.GithubUser;
            Image = user.Image; 
        }

        public UserDto() { }
    }
}
