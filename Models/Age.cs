using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class Age
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Seokeyword { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
