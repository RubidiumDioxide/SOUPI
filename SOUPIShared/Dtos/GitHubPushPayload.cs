namespace SOUPIShared.Dtos
{
    public class GitHubPushPayload
    {
        public string Ref { get; set; } = default!;  // e.g., "refs/heads/main"
        public RepositoryInfo Repository { get; set; } = default!;
        public List<CommitInfo> Commits { get; set; } = default!; 

        public class RepositoryInfo
        {
            public long Id { get; set; }
            public string Name { get; set; } = default!;
            public string FullName { get; set; } = default!; 
        }

        public class CommitInfo
        {
            public string Message { get; set; } = default!; 
            public string Url { get; set; } = default!; 
        }
    }
}
