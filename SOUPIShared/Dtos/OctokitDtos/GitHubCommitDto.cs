using Octokit; 


namespace SOUPIShared.Dtos.OctokitDtos
{
    public class GitHubCommitDto
    {
        public string Sha { get; set; } = default!;
        
        // derived from Octokit.Author
        public string AuthorLogin { get; set; } = default!; 

        // derived from Octokit.Commit
        public string CommitMessage { get; set; } = default!;

        public string HtmlUrl { get; set; } = default!; 

        // derived from Octokit.GitHubCommitFile
        public IReadOnlyList<GitHubCommitFileDto> Files { get; set; } = default!; 


        public GitHubCommitDto(Octokit.GitHubCommit gitHubCommit) 
        {
            Sha = gitHubCommit.Sha;
            AuthorLogin = gitHubCommit.Author.Login ?? "Anonymous";
            CommitMessage = gitHubCommit.Commit.Message;
            HtmlUrl = gitHubCommit.HtmlUrl;
            Files = gitHubCommit.Files.Select(f => new GitHubCommitFileDto(f)).ToList().AsReadOnly();  
        } 

        public GitHubCommitDto() { } 
    }
}