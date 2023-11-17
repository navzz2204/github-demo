using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime DateOrder { get; set; }

    public DateTime DateShip { get; set; }

    public int Offer { get; set; }

    public bool IsPaid { get; set; }

    public bool IsCancel { get; set; }

    public bool IsDelete { get; set; }

    public bool IsDelivere { get; set; }

    public bool IsApproved { get; set; }

    public bool IsReceived { get; set; }

    public decimal? Total { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; } = null!;
}
