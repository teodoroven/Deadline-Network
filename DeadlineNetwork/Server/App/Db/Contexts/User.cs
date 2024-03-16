using System;
using System.Collections.Generic;

namespace Server;

public partial class User
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string LoginHash { get; set; }

    public required string PasswordHash { get; set; }
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
