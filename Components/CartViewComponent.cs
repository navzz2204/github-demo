using Microsoft.AspNetCore.Mvc;
using ToyStoreOnlineWeb.Data.Repository;
using ToyStoreOnlineWeb.Models;
using ToyStoreOnlineWeb.Service;
using ToyStoreOnlineWeb.ViewModels;
using ToyStoreOnlineWeb.Controllers;

public class CartViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(List<ItemCart> Carts, double totalQuantity, string totalPrice)
    {
        // Tạo một ViewModel để chứa dữ liệu
        CartViewModel viewModel = new CartViewModel
        {
            Carts= Carts,
            TotalQuantity = totalQuantity,
            TotalPrice = totalPrice
        };

        return View(viewModel);
    }


}