using Microsoft.AspNetCore.Mvc;
using ToyStoreOnlineWeb.Models;
using ToyStoreOnlineWeb.Service;
using ToyStoreOnlineWeb.ViewModels;
public class MenuViewComponent : ViewComponent
{
    private readonly IProductCategoryService _productCategoryService;
    private readonly IProducerService _producerService;
    private readonly IAgeService _ageService;
    private readonly IProductCategoryParentService _productCategoryParentService;
    private readonly IGenderService _genderService;

    // Khởi tạo với dependency injection
    public MenuViewComponent(
        IProductCategoryService productCategoryService,
        IProducerService producerService,
        IAgeService ageService,
        IProductCategoryParentService productCategoryParentService,
        IGenderService genderService)
    {
        _productCategoryService = productCategoryService;
        _producerService = producerService;
        _ageService = ageService;
        _productCategoryParentService = productCategoryParentService;
        _genderService = genderService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new MenuViewModel
        {
            ProductCategories = _productCategoryService.GetProductCategoryHome(),
            Producers = _producerService.GetProducerList(),
            Ages = _ageService.GetAgeList(),
            ProductCategoryParents = _productCategoryParentService.GetProductCategoryParentList(),
            Genders = _genderService.GetGenderList()
        };

        return View(model);
    }
    

}