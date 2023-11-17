using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using ToyStoreOnlineWeb.Models;
using ToyStoreOnlineWeb.Service;
using X.PagedList.Mvc.Core;
public class ProductViewComponent : ViewComponent
{
    private readonly IRatingService _ratingService;

    public ProductViewComponent(IRatingService ratingService, IProductService productService)
    {
        _ratingService = ratingService;
  
    }

    public async Task<IViewComponentResult> InvokeAsync(Product product)
    {
        // Get rating
        var rating = _ratingService.GetRating(product.Id);
        ViewData["Rating"] = rating; // Sử dụng ViewData để truyền dữ liệu tới view

        return View(product);
    }
}