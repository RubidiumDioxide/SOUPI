using System;
using System.Collections.Generic;

namespace SOUPIShared.Models;

public partial class User
{
    public Guid Id { get; set; } 

    public string Login { get; set; } = null!; 


    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
