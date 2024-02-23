using System;
using System.Collections.Generic;

namespace Server;

public partial class UserCredential
{
    public int UserId { get; set; }

    public required string LoginHash { get; set; }

    public required string PasswordHash { get; set; }

    public virtual User User { get; set; } = null!;
}
