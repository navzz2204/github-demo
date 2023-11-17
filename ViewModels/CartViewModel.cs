using ToyStoreOnlineWeb.Models;
namespace ToyStoreOnlineWeb.ViewModels
{
    public class CartViewModel
    {
     
        public List<ItemCart>? Carts { get; set; }
        public double TotalQuantity { get; set; }
        public string? TotalPrice { get; set; }
      
    }

}
