using System;
using System.Collections.Generic;

namespace SOUPIShared.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int RecieverId { get; set; }

    public int SenderId { get; set; }

    public string Text { get; set; } = null!;

    public string Link { get; set; } = null!;

    public virtual User Reciever { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
