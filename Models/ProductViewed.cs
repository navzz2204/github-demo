using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class ProductViewed
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public DateTime Date { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
