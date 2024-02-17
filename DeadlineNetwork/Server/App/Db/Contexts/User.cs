using System;
using System.Collections.Generic;

namespace Server;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual UserCredential? UserCredential { get; set; }
}
