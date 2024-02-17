using System;
using System.Collections.Generic;

namespace Server;

public partial class Descipline
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int GroupId { get; set; }
    
    public string? Comment { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    // Aboba
}
