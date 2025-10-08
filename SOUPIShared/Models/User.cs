namespace SOUPIShared.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? GithubUser { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<Notification> NotificationRecievers { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> NotificationSenders { get; set; } = new List<Notification>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Task> TaskAssignees { get; set; } = new List<Task>();

    public virtual ICollection<Task> TaskCreators { get; set; } = new List<Task>();

    public virtual ICollection<TeamMember> TeamMemberManagers { get; set; } = new List<TeamMember>();

    public virtual ICollection<TeamMember> TeamMemberUseds { get; set; } = new List<TeamMember>();
}
