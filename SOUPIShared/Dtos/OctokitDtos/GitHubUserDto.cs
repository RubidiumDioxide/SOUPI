using Octokit; 


namespace SOUPIShared.Dtos.OctokitDtos
{
    public class GitHubUserDto
    {
        public string AvatarUrl { get; set; } = default!;

        public string Bio { get; set; } = default!; 
      
        public string HtmlUrl { get; set; } = default!;

        public string Login { get; set; } = default!;

        public string Name { get; set; } = default!;


        public GitHubUserDto(Octokit.User user)
        {
            AvatarUrl = user.AvatarUrl;
            Bio = user.Bio; 
            HtmlUrl = user.HtmlUrl; 
            Login = user.Login; 
            Name = user.Name; 
        }

        public GitHubUserDto() { } 
    }
}