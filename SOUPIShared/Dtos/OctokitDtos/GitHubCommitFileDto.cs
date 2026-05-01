using System.Diagnostics.CodeAnalysis;


namespace SOUPIShared.Dtos.OctokitDtos
{
    public class GitHubCommitFileDto
    {
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly")]
        public string Filename { get; set; } = default!; 

        public int Additions { get; set; }

        public int Deletions { get; set; }

        public int Changes { get; set; }

        public string Status { get; set; } = default!;

        public string Patch { get; set; } = default!; 


        public GitHubCommitFileDto(Octokit.GitHubCommitFile file) 
        {
            Filename = file.Filename;  
            Additions = file.Additions; 
            Deletions = file.Deletions; 
            Changes = file.Changes; 
            Status = file.Status; 
            Patch = file.Patch; 
        }

        public GitHubCommitFileDto() { } 
    }
}
