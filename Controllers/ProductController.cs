using X.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using ToyStoreOnlineWeb.Models;
using ToyStoreOnlineWeb.Service;
using ToyStoreOnlineWeb.Infrastructure;

namespace ToyStoreOnlineWeb.Controllers
{
    public class ProductController : Controller
    {
        #region Initialize
        private IProductService _productService;
        private IProducerService _producerService;
        private IProductCategoryService _productCategoryService;
        private IAgeService _ageService;
        private IProductCategoryParentService _productCategoryParentService;
        private IGenderService _genderService;
        private IUserService _userService;
        private IProductViewedService _productViewedService;
        private IRatingService _ratingService;
        private IOrderDetailService _orderDetailService;

        ToyStoreDbContext context = new ToyStoreDbContext();

        public ProductController(IProductService productService, IProducerService producerService, IProductCategoryService productCategoryService, IAgeService ageService, IProductCategoryParentService productCategoryParentService, IGenderService genderService, IUserService userService, IProductViewedService productViewedService, IRatingService ratingService, IOrderDetailService orderDetailService)
        {
            _productService = productService;
            _producerService = producerService;
            _productCategoryService = productCategoryService;
            _ageService = ageService;
            _productCategoryParentService = productCategoryParentService;
            _genderService = genderService;
            _userService = userService;
            _productViewedService = productViewedService;
            _ratingService = ratingService;
            _orderDetailService = orderDetailService;
        }
        #endregion
        public ActionResult Search(string keyword, int page = 1)
        {
            var listProduct = _productService.GetProductList(keyword);
            ViewBag.ListProduct = listProduct;
            if (keyword == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.Keyword = keyword;
            var products = _productService.GetProductList(keyword);
            PagedList<Product> listProductSearch = new PagedList<Product>(products, page, 12);
            return View(listProductSearch);
        }
        public ActionResult Details(int ID)
        {
            var product = _productService.GetByID(ID);
            var listProduct = _productService.GetProductListByCategory(product.CategoryId).Where(x => x.Id != ID);
            ViewBag.ListProduct = listProduct;

            IEnumerable<User> listUser = _userService.GetList();
            ViewBag.UserList = listUser;

            if (HttpContext.Session.GetString("User") != null)
            {
                var user = HttpContext.Session.GetObject<User>("User");
                _productViewedService.AddProductViewByUser(ID, user.Id);
            }
            //Add view count
            if (Request.Cookies["ViewedPage"] != null)
            {
                string cookieKey = $"ID_{ID}";
                if (Request.Cookies[cookieKey] == null)
                {
                    // Tạo một cookie mới và cài đặt giá trị cho nó
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddMinutes(1);

                    Response.Cookies.Append(cookieKey, "1", options);

                    _productService.AddViewCount(ID);
                }
            }
            else
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddMinutes(1);
                // Đặt giá trị cookie dựa trên ID sản phẩm.
                Response.Cookies.Append($"ID_{ID}", "1", options);

                _productService.AddViewCount(ID);
            }
            //Get rating
            ViewBag.Rating = _ratingService.GetRating(ID);
            //Get list rating
            ViewBag.ListRating = _ratingService.GetListRating(ID);
            return View(product);
        }
        [HttpGet]
        public ActionResult Ages(int ID, string keyword = "", int page = 1)
        {
            var listProduct = _productService.GetProductList().OrderByDescending(x => x.ViewCount).Take(5);
            ViewBag.ListProduct = listProduct;

            ViewBag.ageID = ID;
            Age ages = _ageService.GetAgeByID(ID);
            ViewBag.Name = "Độ tuổi " + ages.Name;

            PagedList<Product> listProductPaging;
            if (keyword != "")
            {
                IEnumerable<Product> products = _productService.GetProductListByAge(ID).Where(x => x.Name.Contains(keyword));
                listProductPaging = new PagedList<Product>(products, page, 12);
            }
            else
            {
                IEnumerable<Product> products = _productService.GetProductListByAge(ID);
                listProductPaging = new PagedList<Product>(products, page, 12);
            }
            return View(listProductPaging);
        }
        public ActionResult Producer(int ID, int page = 1)
        {
            var listProduct = _productService.GetProductList().OrderByDescending(x => x.ViewCount).Take(5);
            ViewBag.ListProduct = listProduct;

            ViewBag.producerID = ID;
            Producer producer = _producerService.GetByID(ID);
            ViewBag.Name = "Thương hiệu " + producer.Name;

            PagedList<Product> listProductPaging;
            IEnumerable<Product> products = _productService.GetProductListByProducer(ID);
            listProductPaging = new PagedList<Product>(products, page, 12);
            return View(listProductPaging);
        }
        public ActionResult ProductCategory(int ID, int page = 1)
        {
            var listProduct = _productService.GetProductList().OrderByDescending(x => x.ViewCount).Take(5);
            ViewBag.ListProduct = listProduct;

            ViewBag.productCategoryID = ID;
            ProductCategory productCategory = _productCategoryService.GetByID(ID);
            ViewBag.Name = "Danh mục " + productCategory.Name;

            PagedList<Product> listProductPaging;
            IEnumerable<Product> products = _productService.GetProductListByCategory(ID);
            listProductPaging = new PagedList<Product>(products, page, 9);
            return View(listProductPaging);
        }
        public ActionResult ProductCategoryParent(int ID, int page = 1)
        {
            var listProduct = _productService.GetProductList().OrderByDescending(x => x.ViewCount).Take(5);
            ViewBag.ListProduct = listProduct;

            ViewBag.productCategoryParentID = ID;
            ProductCategoryParent productCategoryParent = _productCategoryParentService.GetByID(ID);
            ViewBag.Name = "Danh mục gốc " + productCategoryParent.Name;

            PagedList<Product> listProductPaging;
            IEnumerable<Product> products = _productService.GetProductListByCategoryParent(ID);
            listProductPaging = new PagedList<Product>(products, page, 12);
            return View(listProductPaging);
        }
        public ActionResult Gender(int ID, int page = 1)
        {
            var listProduct = _productService.GetProductList().OrderByDescending(x => x.ViewCount).Take(5);
            ViewBag.ListProduct = listProduct;

            ViewBag.GenderID = ID;
            Gender gender = _genderService.GetGenderByID(ID);
            ViewBag.Name = "Giới tính " + gender.Name;

            PagedList<Product> listProductPaging;
            IEnumerable<Product> products;
            if (ID != 3)
            {
                products = _productService.GetProductListByGender(ID);
            }
            else
            {
                products = _productService.GetProductList();
            }
            listProductPaging = new PagedList<Product>(products, page, 12);
            return View(listProductPaging);
        }
        public ActionResult NewProduct(int page = 1)
        {
            var listProduct = _productService.GetProductList().OrderByDescending(x => x.ViewCount).Take(5);
            ViewBag.ListProduct = listProduct;

            PagedList<Product> listProductPaging;
            IEnumerable<Product> products = _productService.GetProductListLast();
            listProductPaging = new PagedList<Product>(products, page, 3);
            return View(listProductPaging);
        }

        public ActionResult ProductViewed(int page = 1)
        {
            var listProduct = _productService.GetProductList().OrderByDescending(x => x.ViewCount).Take(5);
            ViewBag.ListProduct = listProduct;

            var user = HttpContext.Session.GetObject<User>("User");
            PagedList<Product> listProductPaging;
            IEnumerable<Product> products = _productService.GetProductListViewedByUserID(user.Id);
            listProductPaging = new PagedList<Product>(products, page, 10);
            return View(listProductPaging);
        }
        public ActionResult DeleteHistoryView()
        {
            var user = HttpContext.Session.GetObject<User>("User");
            _productViewedService.Delete(user.Id);
            TempData["DeleteHistoryView"] = "Sucess";
            return RedirectToAction("ProductViewed");
        }
        public ActionResult ProductPartial(Product product)
        {
            //Get rating
            ViewBag.Rating = _ratingService.GetRating(product.Id);
            return PartialView(product);
        }
        public PartialViewResult FilterProductList(string type, int ID, int min = 0, int max = 0, int discount = 0, int page = 1)
        {
            PagedList<Product> listProductPaging = null;
            if (type == "Ages")
            {
                ViewBag.Name = "Độ tuổi " + _ageService.GetAgeByID(ID).Name;
                IEnumerable<Product> products = _productService.GetProductFilterByAges(ID, min, max, discount);
                listProductPaging = new PagedList<Product>(products, page, 2);
            }

            ViewBag.Min = min;
            ViewBag.Max = max;
            ViewBag.Type = type;
            ViewBag.ID = ID;
            //Check null
            if (listProductPaging != null)
            {
                //Return view
                return PartialView("ProductContainerPartial", listProductPaging);
            }
            else
            {
                //return 404
                return null;
            }
        }
        [HttpGet]
   
        public ActionResult CommentPartial(int ID)
        {
            IEnumerable<User> listUser = _userService.GetList();
            ViewBag.UserList = listUser;
            return PartialView("_CommentPartial");
        }
   
        [HttpPost]
        public ActionResult Rating(Rating rating, int OrderDetailID)
        {
            var user = HttpContext.Session.GetObject<User>("User");
            rating.UserId = user.Id;
            _ratingService.AddRating(rating);
            _orderDetailService.SetIsRating(OrderDetailID);
            return RedirectToAction("Details", "Product", new { ID = rating.ProductId });
        }
       
       
     
    }
}