namespace SOUPIShared.Models;

public partial class TeamMember
{
    public int Id { get; set; }

    public int UsedId { get; set; }

    public int ProjectId { get; set; }

    public string? Role { get; set; }

    public int? ManagerId { get; set; }

    public virtual User? Manager { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User Used { get; set; } = null!;
}
