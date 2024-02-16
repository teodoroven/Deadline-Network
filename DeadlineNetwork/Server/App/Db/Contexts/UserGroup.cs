using System;
using System.Collections.Generic;

namespace Server.App.Db.Contexts;

public partial class UserGroup
{
    public Guid UserId { get; set; }

    public Guid GroupId { get; set; }

    public bool? IsOwner { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
