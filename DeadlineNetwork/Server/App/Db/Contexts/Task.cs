using System;
using System.Collections.Generic;

namespace Server;

public partial class Task
{
    public int Id { get; set; }

    public int WhoAdded { get; set; }

    public int DesciplineId { get; set; }

    public DateTime Deadline { get; set; }

    public DateTime Created { get; set; }
    public required string Comment { get; set; }

    public virtual Descipline Descipline { get; set; } = null!;

    public virtual User WhoAddedNavigation { get; set; } = null!;
}
