using System;
using System.Collections.Generic;

namespace Server.App.Db.Contexts;

public partial class User
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual UserCredential? UserCredential { get; set; }
}
