using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class Rating
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public int Star { get; set; }

    public string? Content { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
