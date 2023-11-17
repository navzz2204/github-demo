using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class User
{
    public int Id { get; set; }

    public int UserTypeId { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? Capcha { get; set; }

    public decimal? AmountPurchased { get; set; }

    public string? Avatar { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<ItemCart> ItemCarts { get; set; } = new List<ItemCart>();

    public virtual ICollection<Message> MessageFromUsers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageToUsers { get; set; } = new List<Message>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductViewed> ProductVieweds { get; set; } = new List<ProductViewed>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual UserType UserType { get; set; } = null!;

    public virtual ICollection<UsersSpin> UsersSpins { get; set; } = new List<UsersSpin>();
}
