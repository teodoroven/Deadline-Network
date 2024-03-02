using System;
using System.Collections.Generic;

namespace Server;

public partial class Group
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string PasswordHash { get; set; }

    public virtual ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();
}
