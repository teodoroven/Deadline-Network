using System;
using System.Collections.Generic;

namespace Server;

public partial class Group
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? PasswordHash { get; set; }

    public virtual ICollection<Descipline> Desciplines { get; set; } = new List<Descipline>();
}
