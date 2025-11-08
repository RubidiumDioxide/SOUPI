using System;
using System.Collections.Generic;

namespace SOUPIShared.Models;

public partial class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? GithubRepository { get; set; }

    public int CreatorId { get; set; }

    public string? Image { get; set; }

    public virtual User Creator { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
