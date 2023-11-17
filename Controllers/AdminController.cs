using Microsoft.AspNetCore.Mvc;

namespace ToyStoreOnlineWeb.Controllers
{
    public class AdminController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
    }
}
