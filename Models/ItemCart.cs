using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace ToyStoreOnlineWeb.Models;

public partial class ItemCart
{
    public ItemCart(int iID)
    {
        ToyStoreDbContext db = new ToyStoreDbContext();
        this.ProductId = iID;
        Product product = db.Products.Single(n => n.Id == iID);
        this.Name = product.Name;
        this.Image = product.Image1;
        this.Price = (decimal)product.PromotionPrice;
        this.Quantity = 1;
        this.Total = Price * Quantity;
    }
    public ItemCart()
    {

    }
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }
   
    public decimal Total { get; set; }
   
    public string Name { get; set; }
  
    public string Image { get; set; }
    
    public decimal Price { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
