using Octokit; 


namespace SOUPIShared.Dtos.OctokitDtos
{
    public class GitHubRepositoryDto
    {
        public string HtmlUrl { get; set; } = default!;

        // derived from Octokit.Repository.Owner
        public string OwnerLogin { get; set; } = default!; 

        public string Name { get; set; } = default!; 

        // inlcudes the owner (owner/name)
        public string FullName { get; set; } = default!;

        public string Description { get; set; } = default!;

        public bool Private { get; set; }


        public GitHubRepositoryDto(Octokit.Repository repository)
        {
            HtmlUrl = repository.HtmlUrl;
            OwnerLogin = repository.Owner.Login;  
            Name = repository.Name; 
            FullName = repository.FullName; 
            Description = repository.Description; 
            Private = repository.Private; 
        }

        public GitHubRepositoryDto() { } 
    }
}