using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class UserType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Decentralization> Decentralizations { get; set; } = new List<Decentralization>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
