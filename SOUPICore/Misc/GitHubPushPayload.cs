using System.ComponentModel.DataAnnotations; 


namespace SOUPICore.Misc
{
    public class GitHubPushPayload
    {
        public string? Ref { get; set; } // e.g., "refs/heads/main"
        [Required]
        public RepositoryInfo Repository { get; set; } = default!;
        public List<CommitInfo>? Commits { get; set; } 

        public class RepositoryInfo
        {
            [Required]
            public long Id { get; set; }
            [Required]
            public string Name { get; set; } = default!;
            [Required]
            public string FullName { get; set; } = default!; 
        }

        public class CommitInfo
        {
            [Required]
            public string Id { get; set; } = default!;
            [Required]
            public string Message { get; set; } = default!; 
            [Required] 
            public string Url { get; set; } = default!;
            [Required]
            public AuthorInfo Author { get; set; } = default!; 

            public class AuthorInfo
            {
                [Required]
                public string Name { get; set; } = default!;  // Full name 
                [Required] 
                public string Username { get; set; } = default!; // GitHub handle
            }
        }
    }
}
