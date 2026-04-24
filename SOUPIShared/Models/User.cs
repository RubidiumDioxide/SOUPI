using System.ComponentModel.DataAnnotations;
using SOUPIShared.Attributes; 


namespace SOUPIShared.Models;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [ValidGitHubUsername]
    public string Login { get; set; } = default!;


    public virtual List<Project> Projects { get; set; } = default!;
    public virtual List<TeamMember> TeamMembers { get; set; } = default!; 
    public virtual List<Notification> ReceivedNotifications { get; set; } = default!;
    public virtual List<Notification> SentNotifications { get; set; } = default!;      
    public virtual List<Job> CreatedJobs { get; set; } = default!; 
}
