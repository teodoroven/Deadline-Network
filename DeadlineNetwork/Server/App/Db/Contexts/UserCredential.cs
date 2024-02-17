using System;
using System.Collections.Generic;

namespace Server.App.Db.Contexts;

public partial class UserCredential
{
    public Guid UserId { get; set; }

    public string? LoginHash { get; set; }

    public string? PasswordHash { get; set; }

    public virtual User User { get; set; } = null!;
}
