using System;
using System.Collections.Generic;

namespace Server.App.Db.Contexts;

public partial class Descipline
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid? GroupId { get; set; }

    public string? Comment { get; set; }

    public virtual Group? Group { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
