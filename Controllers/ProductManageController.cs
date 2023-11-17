using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using ToyStoreOnlineWeb.Service;
using ToyStoreOnlineWeb.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;


namespace ToyStoreOnlineWeb.Controllers
{
   
    public class ProductManageController : Controller
    {
        #region Initialize
        private IFileStorageService _fileStorageService;
        private IProductService _productService;
        private IProductCategoryService _productCategoryService;
        private IProducerService _producerService;
        private IAgeService _ageService;
        private IGenderService _genderService;

        public ProductManageController(IFileStorageService fileStorageService,
            IProductService productService,
            IProductCategoryService productCategoryService,
            IProducerService producerService,
            IAgeService ageService,
            IGenderService genderService)
        {
            _fileStorageService = fileStorageService;
            _productService = productService;
            _productCategoryService = productCategoryService;
            _producerService = producerService;
            _ageService = ageService;
            _genderService = genderService;
        }
        #endregion
        // GET: Product
        [HttpGet]
        public ActionResult Index(int? page = 1, string keyword = "")
        {
          //  if (Session["User"] == null)
          //  {
          //      return RedirectToAction("Login");
          //  }
            //Get data for DropdownList
            ViewBag.CategoryID = new SelectList(_productCategoryService.GetProductCategoryList().OrderBy(x => x.Name), "Id", "Name");
            ViewBag.ProducerID = new SelectList(_producerService.GetProducerList().OrderBy(x => x.Name), "Id", "Name");
            ViewBag.AgeID = new SelectList(_ageService.GetAgeList(), "Id", "Name");
            ViewBag.GenderID = new SelectList(_genderService.GetGenderList(), "Id", "Name");

            ViewBag.CategoryIDEdit = ViewBag.CategoryID;
            ViewBag.ProducerIDEdit = ViewBag.ProducerID;
            ViewBag.AgeIDEdit = ViewBag.AgeID;
            ViewBag.GenderIDEdit = ViewBag.GenderID;

            ViewBag.CategoryIDDetail = ViewBag.CategoryID;
            ViewBag.ProducerIDDetail = ViewBag.ProducerID;
            ViewBag.AgeIDDetail = ViewBag.AgeID;
            ViewBag.GenderIDDetail = ViewBag.GenderID;

            int pageSize = 5;
            var pageNumber = page ?? 1; // Sử dụng giá trị mặc định là 1 nếu page là null
            //Get proudct category list
            IQueryable<Product> products;

            if (keyword != "")
            {
                products = _productService.GetProductListForManage()
                    .Where(x => x.Name.Contains(keyword))
                    .OrderByDescending(x => x.LastUpdatedDate.Date)
                    .AsQueryable(); // Chuyển đổi IOrderedEnumerable thành IQueryable
                ViewBag.Products = products;
            }
            else
            {
                products = _productService.GetProductListForManage()
                    .OrderByDescending(x => x.LastUpdatedDate.Date)
                    .AsQueryable(); // Chuyển đổi IOrderedEnumerable thành IQueryable
                ViewBag.Products = products;
            }

            var listProduct = products.ToPagedList(pageNumber, pageSize);
            ViewBag.KeyWord = keyword;
            ViewBag.Page = pageNumber;
            if (listProduct != null)
            {
                ViewBag.Page = page;
                return View(listProduct);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
      public IActionResult ListName(string Prefix)
        {
            List<string> names = _productService.GetProductListName(Prefix).ToList();
            return Json(names);
        }
        [HttpPost]
        public ActionResult List(string keyword)
        {
          //  if (Session["User"] == null)
          //  {
          //      return RedirectToAction("Login");
         //   }
            int pageSize = 5;
            if (keyword == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Get proudct category list with keyword
            var products = _productService.GetProductListForManage(keyword);
            PagedList<Product> listProduct = new PagedList<Product>(products, 1, pageSize);
            //Check null
            if (listProduct != null)
            {
                ViewBag.message = "Hiển thị kết quả tìm kiếm với '" + keyword + "";
                //Return view
                return View(listProduct);
            }
            else
            {
                //return 404
                return NotFound();
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
      
            //Get data for DropdownList
            ViewBag.CategoryID = new SelectList(_productCategoryService.GetProductCategoryList().OrderBy(x => x.Name), "Id", "Name");
            ViewBag.ProducerID = new SelectList(_producerService.GetProducerList().OrderBy(x => x.Name), "Id", "Name");
            ViewBag.AgeID = new SelectList(_ageService.GetAgeList(), "Id", "Name");
            ViewBag.GenderID = new SelectList(_genderService.GetGenderList(), "Id", "Name");
            //Return view
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(Product product, IFormFile[] ImageUpload, int page)
        {
          

            //Declare a errorCount
            int errorCount = 0;
            List<string> fileNames = new List<string>();

            for (int i = 0; i < ImageUpload.Length; i++)
            {
                if (ImageUpload[i] != null && ImageUpload[i].Length > 0)
                {
                    // Kiểm tra định dạng hình ảnh
                    if (ImageUpload[i].ContentType != "image/jpeg" && ImageUpload[i].ContentType != "image/png" && ImageUpload[i].ContentType != "image/gif")
                    {
                        // Thêm thông báo lỗi vào ModelState
                        ModelState.AddModelError("ImageUpload", "Hình ảnh không hợp lệ");
                        // Tăng biến đếm lỗi
                        errorCount++;
                    }
                    else
                    {
                        // Lưu trữ tệp tải lên
                        var fileName = await _fileStorageService.SaveFileAsync(ImageUpload[i]);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            fileNames.Add(fileName);
                        }
                        else
                        {
                            // Xử lý lỗi nếu việc lưu tệp thất bại
                            ModelState.AddModelError("ImageUpload", "Lỗi khi lưu hình ảnh");
                            errorCount++;
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("ImageUpload", "Chưa chọn hình ảnh.");
                     errorCount++;
                    return View(product);
                   
                }
            }

            // Kiểm tra lỗi
            if (errorCount > 0)
            {
                
                // Nếu có lỗi, trả về view với thông báo lỗi
                // Đảm bảo bạn đã thêm thông báo lỗi vào view
                return View(product);
            }

            // Cài đặt giá trị hình ảnh cho sản phẩm
            if (fileNames.Count >= 1) product.Image1 = fileNames[0];
            if (fileNames.Count >= 2) product.Image2 = fileNames[1];
            if (fileNames.Count >= 3) product.Image3 = fileNames[2];
            if (fileNames.Count >= 4) product.Image4 = fileNames[3];

            //Set TempData for checking in view to show swal
            TempData["create"] = "Success";

            // Thêm sản phẩm và lưu vào cơ sở dữ liệu
            _productService.AddProduct(product);

            // Điều hướng đến trang Index
            return RedirectToAction("Index", new { page = 1 });
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
       
            //Get product catetgory
            var product = _productService.GetByID(id);

            //Get data for DropdownList
            ViewBag.CategoryIDEdit = new SelectList(_productCategoryService.GetProductCategoryList().OrderBy(x => x.Name), "Id", "Name", product.CategoryId);
            ViewBag.ProducerIDEdit = new SelectList(_producerService.GetProducerList().OrderBy(x => x.Name), "Id", "Name", product.ProducerId);
            ViewBag.AgeIDEdit = new SelectList(_ageService.GetAgeList(), "Id", "Name", product.AgeId);
            ViewBag.GenderIDEdit = new SelectList(_genderService.GetGenderList(), "Id", "Name", product.GenderId);

            //Check null
            if (product != null)
            {
                //Return view
                return Json(new
                {
                    Id = product.Id,
                    Name = product.Name,
                    CategoryId = product.CategoryId,
                    ProducerId = product.ProducerId,
                    AgeId = product.AgeId,
                    GenderId = product.GenderId, 
                    Image1 = product.Image1,
                    Image2 = product.Image2,
                    Image3 = product.Image3,
                    Price = product.Price,
                    Discount = product.Discount,
                    Description = product.Description,
                    IsActive = product.IsActive,
                    ViewCount = product.ViewCount,
                    PurchasedCount = product.PurchasedCount,
                    Quantity = product.Quantity,
                    Seokeyword = product.Seokeyword,
                    status = true
                });
            }
            else
            {
                //Return 404
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<ActionResult> Edit(Product product, IFormFile[] ImageUpload, int page, int CategoryIDEdit, int ProducerIDEdit, int AgeIDEdit, int GenderIDEdit)
        {
        
            //Get data for DropdownList
            ViewBag.CategoryID = new SelectList(_productCategoryService.GetProductCategoryList().OrderBy(x => x.Name), "Id", "Name", product.CategoryId);
            ViewBag.ProducerID = new SelectList(_producerService.GetProducerList().OrderBy(x => x.Name), "Id", "Name", product.ProducerId);
            ViewBag.AgeID = new SelectList(_ageService.GetAgeList(), "Id", "Name", product.AgeId);
            ViewBag.GenderID = new SelectList(_genderService.GetGenderList(), "Id", "Name", product.GenderId);

            //Declare a errorCount
            int errorCount = 0;
            List<string> fileNames = new List<string>();

            for (int i = 0; i < ImageUpload.Length; i++)
            {
                if (ImageUpload[i] != null && ImageUpload[i].Length > 0)
                {
                    // Kiểm tra định dạng hình ảnh
                    if (ImageUpload[i].ContentType != "image/jpeg" && ImageUpload[i].ContentType != "image/png" && ImageUpload[i].ContentType != "image/gif")
                    {
                        // Thêm thông báo lỗi vào ModelState
                        ModelState.AddModelError("ImageUpload", "Hình ảnh không hợp lệ");
                        // Tăng biến đếm lỗi
                        errorCount++;
                    }
                    else
                    {
                        // Lưu trữ tệp tải lên
                        var fileName = await _fileStorageService.SaveFileAsync(ImageUpload[i]);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            fileNames.Add(fileName);
                        }
                        else
                        {
                            // Xử lý lỗi nếu việc lưu tệp thất bại
                            ModelState.AddModelError("ImageUpload", "Lỗi khi lưu hình ảnh");
                            errorCount++;
                        }
                    }
                }
            }

            // Kiểm tra lỗi
            if (errorCount > 0)
            {
                // Nếu có lỗi, trả về view với thông báo lỗi
                
                return View(product);
            }

            // Cài đặt giá trị hình ảnh cho sản phẩm
            if (fileNames.Count >= 1) product.Image1 = fileNames[0];
            if (fileNames.Count >= 2) product.Image2 = fileNames[1];
            if (fileNames.Count >= 3) product.Image3 = fileNames[2];
            if (fileNames.Count >= 4) product.Image4 = fileNames[3];

            //Set TempData for checking in view to show swal
            TempData["edit"] = "Success";
            //Update productCategory
            product.CategoryId = CategoryIDEdit;
            product.ProducerId = ProducerIDEdit;
            product.AgeId = AgeIDEdit;
            product.GenderId = GenderIDEdit;
            _productService.UpdateProduct(product);
            string url = HttpContext.Request.GetEncodedUrl();
            return RedirectToAction("Index", new { page = page });
        }
        public void Block(int id)
        {
            //Get productCategory by ID
            var product = _productService.GetByID(id);
            //Delete productCategory
            _productService.DeleteProduct(product);
        }
        public void Active(int id)
        {
            //Get productCategory by ID
            var product = _productService.GetByID(id);
            //Active productCategory
            _productService.ActiveProduct(product);
        }
        public ActionResult ProductActivePartial(int ID)
        {
            //if (Session["User"] == null)
           // {
           //     return RedirectToAction("Login");
          //  }
            return PartialView("ProductActivePartial", _productService.GetByID(ID));
        }
        [HttpPost]
        public JsonResult CheckName(string name, int id = 0)
        {
            Product product = _productService.GetByName(name);
            if (product != null)
            {
                if (product.Id == id)
                {
                    return Json(new
                    {
                        status = true
                    });
                }
                else
                {
                    if (_productService.CheckName(name))
                    {
                        return Json(new
                        {
                            status = true
                        });
                    }
                }
            }
            if (_productService.CheckName(name))
            {
                return Json(new
                {
                    status = true
                });
            }
            return Json(new
            {
                status = false
            });
        }
    }
}



