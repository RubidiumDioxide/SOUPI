namespace SOUPIShared.Dtos
{
    public class GitHubPushPayload
    {
        public string? Ref { get; set; } // e.g., "refs/heads/main"
        public RepositoryInfo Repository { get; set; } = default!;
        public List<CommitInfo>? Commits { get; set; } 

        public class RepositoryInfo
        {
            public long Id { get; set; }
            public string Name { get; set; } = default!;
            public string FullName { get; set; } = default!; 
        }

        public class CommitInfo
        {
            public string Id { get; set; } = default!; 
            public string Message { get; set; } = default!; 
            public string Url { get; set; } = default!;
            public AuthorInfo Author { get; set; } = default!; 

            public class AuthorInfo
            {
                public string Name { get; set; } = default!;  // Full name
                public string Username { get; set; } = default!; // GitHub handle
            }
        }
    }
}
