using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class Product
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Product()
    {
       
        this.ItemCarts = new HashSet<ItemCart>();
        this.OrderDetails = new HashSet<OrderDetail>();
        this.ProductVieweds = new HashSet<ProductViewed>();
       
        this.Ratings = new HashSet<Rating>();
    }
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? Image1 { get; set; }

    public string? Image2 { get; set; }

    public string? Image3 { get; set; }

    public string? Image4 { get; set; }

    public decimal Price { get; set; }

    public decimal PromotionPrice { get; set; }

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public int? ViewCount { get; set; }

    public int? PurchasedCount { get; set; }

    public int ProducerId { get; set; }

    public bool IsActive { get; set; }

    public DateTime LastUpdatedDate { get; set; }

    public int AgeId { get; set; }

    public int GenderId { get; set; }

    public int Discount { get; set; }

    public string? Seokeyword { get; set; }

    public virtual Age Age { get; set; } = null!;

    public virtual ProductCategory Category { get; set; } = null!;

    public virtual Gender Gender { get; set; } = null!;

    public virtual ICollection<ItemCart> ItemCarts { get; set; } = new List<ItemCart>();
  

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Producer Producer { get; set; } = null!;

    public virtual ICollection<ProductViewed> ProductVieweds { get; set; } = new List<ProductViewed>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
