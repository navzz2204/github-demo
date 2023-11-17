using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class Decentralization
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string? Note { get; set; }

    public int UserTypeId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual UserType UserType { get; set; } = null!;
}
