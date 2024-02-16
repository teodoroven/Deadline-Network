using System;
using System.Collections.Generic;

namespace Server.App.Db.Contexts;

public partial class Group
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? PasswordHash { get; set; }

    public virtual ICollection<Descipline> Desciplines { get; set; } = new List<Descipline>();
}
