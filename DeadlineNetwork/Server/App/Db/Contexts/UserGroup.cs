using System;
using System.Collections.Generic;

namespace Server;

public partial class UserGroup
{
    public int UserId { get; set; }

    public int GroupId { get; set; }

    public bool IsOwner { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
