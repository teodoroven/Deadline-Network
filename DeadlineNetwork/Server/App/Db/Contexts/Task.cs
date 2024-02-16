using System;
using System.Collections.Generic;

namespace Server.App.Db.Contexts;

public partial class Task
{
    public Guid Id { get; set; }

    public Guid? WhoAdded { get; set; }

    public Guid? DesciplineId { get; set; }

    public DateTime? Deadline { get; set; }

    public DateTime? Created { get; set; }

    public string? Comment { get; set; }

    public virtual Descipline? Descipline { get; set; }

    public virtual User? WhoAddedNavigation { get; set; }
}
