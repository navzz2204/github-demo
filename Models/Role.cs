using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class Role
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? NameDisplay { get; set; }

    public virtual ICollection<Decentralization> Decentralizations { get; set; } = new List<Decentralization>();
}
