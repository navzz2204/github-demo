using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Collections.Generic;
using ToyStoreOnlineWeb.Models;
using ToyStoreOnlineWeb.Service;
using Microsoft.AspNetCore.Mvc;
using ToyStoreOnlineWeb.Infrastructure;
using System.Web.Helpers;

namespace ToyStoreOnlineWeb.Controllers
{
    
    public class CartController : Controller
    {
        private IProductService _productService;
        private IUserService _userService;
        private IOrderService _orderService;
        private IOrderDetailService _orderDetailService;
        private ICartService _cartService;
        public CartController(IProductService productService, IUserService userService, IOrderService orderService, IOrderDetailService orderDetailService, ICartService cartService)
        {
            _productService = productService;
            _userService = userService;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _cartService = cartService;

        }
       
        // GET: Cart
        public List<ItemCart> GetCart()
        {
            User user = HttpContext.Session.GetObject<User>("User");
            if (user != null)
            {
                if (_cartService.CheckCartUser(user.Id))
                {
                    List<ItemCart> listCart = _cartService.GetCart(user.Id);
                    foreach (ItemCart item in listCart)
                    {
                        if (item.Image == null || item.Image == "")
                        {
                            item.Image = _productService.GetByID(item.ProductId).Image1;
                        }
                        if (item.Price == 0)
                        {
                            item.Price = _productService.GetByID(item.ProductId).PromotionPrice;
                        }
                    }
                    HttpContext.Session.SetObject("Cart", listCart);
                    return listCart;
                }
            }
            else
            {
                List<ItemCart> listCart = HttpContext.Session.GetObject<List<ItemCart>>("Cart");
                //Check null session Cart
                if (listCart == null)
                {
                    //Initialization listCart
                    listCart = new List<ItemCart>();
                    HttpContext.Session.SetObject("Cart", listCart);
                    return listCart;
                }
                return listCart;
            }
            return null;
        }
        [HttpPost]
        public ActionResult AddItemCart(int ID)
        {
            //Check product already exists in DB
            Product product = _productService.GetByID(ID);
            if (product == null)
            {
                //product does not exist
                Response.StatusCode = 404;
                return null;
            }
            //Get cart
            List<ItemCart> listCart = GetCart();
            //If User
            var user = HttpContext.Session.GetObject<User>("User");
            if (user != null)
            {
                //Case 1: If product already exists in Member Cart
                if (_cartService.CheckProductInCart(ID, user.Id))
                {
                    _cartService.AddQuantityProductCartUser(ID, user.Id);
                }
                else
                {
                    //Case 2: If product does not exist in User Cart
                    //Get product
                    ItemCart itemCart = new ItemCart(ID);
                    itemCart.UserId = user.Id;
                    _cartService.AddCartIntoUser(itemCart);
                }
                List<ItemCart> carts = _cartService.GetCart(user.Id);
                foreach (ItemCart item in carts)
                {
                    if (item.Image == null || item.Image == "")
                    {
                        item.Image = _productService.GetByID(item.ProductId).Image1;
                    }
                }
                HttpContext.Session.SetObject("Cart", carts);             
                return ViewComponent("Cart", new { carts = carts, totalQuantity = GetTotalQuanity(), totalPrice = GetTotalPrice().ToString("#,##") });
            }
            else
            {
                if (listCart != null)
                {
                    //Case 1: If product already exists in session Cart
                    ItemCart itemCart = listCart.SingleOrDefault(n => n.ProductId == ID);
                    if (itemCart != null)
                    {
                        //Check inventory before letting customers make a purchase
                        if (product.Quantity < itemCart.Quantity)
                        {
                            return View("ThongBao");
                        }
                        itemCart.Quantity++;
                        itemCart.Total = itemCart.Quantity * product.Price;
                        HttpContext.Session.SetObject("Cart", listCart);
                        return ViewComponent("Cart", new { carts = listCart, totalQuantity = GetTotalQuanity(), totalPrice = GetTotalPrice().ToString("#,##") });
                    }
                    //Case 2: If product does not exist in the Session Cart
                    ItemCart item = new ItemCart(ID);
                    item.Image = _productService.GetByID(item.ProductId).Image1;
                    listCart.Add(item);
                    // Cập nhật lại session
                    HttpContext.Session.SetObject("Cart", listCart);
                }
            }
            ViewBag.TotalQuanity = GetTotalQuanity();
            ViewBag.TotalPrice = GetTotalPrice().ToString("#,##");
            return ViewComponent("Cart", new { carts = listCart, totalQuantity = GetTotalQuanity(), totalPrice = GetTotalPrice().ToString("#,##") });
        }
        [HttpPost]
        public ActionResult CheckQuantityAdd(int ID)
        {
            //Check product already exists in DB
            Product product = _productService.GetByID(ID);
            if (product == null)
            {
                //product does not exist
                Response.StatusCode = 404;
                return null;
            }
            //Get cart
            List<ItemCart> listCart = GetCart();
            //Check quantity
            if (listCart != null)
            {
                int sum = 0;
                foreach (ItemCart item in listCart.Where(x => x.ProductId == ID))
                {
                    sum += item.Quantity;
                }
                if (product.Quantity > sum)
                {
                    return Json(new
                    {
                        status = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = false
                    });
                }
            }
            else
            {
                if (product.Quantity > 0)
                {

                    return Json(new
                    {
                        status = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = false
                    });
                }
            }
        }
        [HttpPost]
        public ActionResult CheckQuantityUpdate(int ID, int Quantity)
        {
            //Check product already exists in DB
            Product product = _productService.GetByID(ID);
            if (product.Quantity >= Quantity)
            {
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }
        }
        public ActionResult CartPartial()
        {
            if (GetTotalQuanity() == 0)
            {
                ViewBag.TotalQuanity = 0;
                ViewBag.TotalPrice = 0;
                return PartialView();
            }
            ViewBag.TotalQuanity = GetTotalQuanity();
            ViewBag.TotalPrice = GetTotalPrice().ToString("#,##");
            return PartialView();
        }
        public double GetTotalQuanity()
        {
             List<ItemCart> listCart = HttpContext.Session.GetObject<List<ItemCart>>("Cart") ?? new List<ItemCart>();
            if (listCart == null)
            {
                return 0;
            }
            return listCart.Sum(n => n.Quantity);
        }
        public decimal GetTotalPrice()
        {
            //Lấy giỏ hàng
            List<ItemCart> listCart = HttpContext.Session.GetObject<List<ItemCart>>("Cart");
            if (listCart == null)
            {
                return 0;
            }
            var f = listCart.Sum(n => n.Total);
            return f;
        }
        public ActionResult Checkout()
        {
            ViewBag.TotalQuantity = GetTotalQuanity();
            var user = HttpContext.Session.GetObject<User>("User");
            try
            {
                HttpContext.Session.SetObject("Cart", GetCart());

                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult EditCart(int ID)
        {
            //Check null session cart
            if (HttpContext.Session.GetString("Cart") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Check whether the product exists in the database or not?
            Product product = _productService.GetByID(ID);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Get cart
            List<ItemCart> listCart = GetCart();
            //Check if the product exists in the shopping cart
            ItemCart item = listCart.SingleOrDefault(n => n.ProductId == ID);
            if (item == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Cart = listCart;
            return Json(new
            {
                ID = item.Id,
                Price = item.Price,
                ProductID = item.ProductId,
                Quantity = item.Quantity,
                Image = item.Image,
                status = true
            });
        }
        [HttpPost]
        public ActionResult EditCart(int ID, int Quantity)
        {
            //Check stock quantity
            Product product = _productService.GetByID(ID);
            //Updated quantity in cart Session
            List<ItemCart> listCart = GetCart();
            //Get products from within listCart to update
            ItemCart itemCartUpdate = listCart.Find(n => n.ProductId == ID);
            itemCartUpdate.Quantity = Quantity;
            itemCartUpdate.Total = itemCartUpdate.Quantity * itemCartUpdate.Price;

            var user = HttpContext.Session.GetObject<User>("User");
            if (user != null)
            {
                //Update Cart Quantity Member
                _cartService.UpdateQuantityCartUser(Quantity, ID, user.Id);
                HttpContext.Session.SetObject("Cart", listCart);

            }

            return RedirectToAction("Checkout");
        }
        [HttpGet]
        public ActionResult RemoveItemCart(int ID)
        {
            //Check null session Cart
            if (HttpContext.Session.GetString("Cart") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Check whether the product exists in the database or not?
            Product product = _productService.GetByID(ID);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Get cart
            List<ItemCart> listCart = GetCart();
            //Check if the product exists in the shopping cart
            ItemCart item = listCart.SingleOrDefault(n => n.ProductId == ID);
            if (item == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Remove item cart
            listCart.Remove(item);
            var user = HttpContext.Session.GetObject<User>("User");
            if (user != null)
            {
                _cartService.RemoveCart(ID, user.Id);
                List<ItemCart> carts = _cartService.GetCart(user.Id);
                HttpContext.Session.SetObject("Cart", carts);

            }
            ViewBag.TotalQuantity = GetTotalQuanity();
            return PartialView("CheckoutPartial");
        }
        [HttpPost]
        public ActionResult AddOrder(User user, int NumberDiscountPass = 0, string CodePass = "", string payment = "")
        {
            //Check null session cart
            if (HttpContext.Session.GetString("Cart") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            User usercheck = new User();
            bool status = true;
            //Is User
            User userOrder = new User();
            if (HttpContext.Session.GetString("User") == null)
            {
                status = false;
                //Check Email & Phone
                if (_userService.CheckEmailPhone(user.Email, user.PhoneNumber))
                {
                    //Insert user into DB
                    userOrder = user;
                    userOrder.Avatar = "user.png";
                    userOrder.EmailConfirmed = false;
                    _userService.Add(userOrder);
                }
                else
                {
                    //Update user in DB
                    userOrder = _userService.GetByPhoneNumber(user.PhoneNumber);
                    userOrder.Address = user.Address;
                    userOrder.FullName = user.FullName;
                    _userService.Update(userOrder);
                }
            }
            //Add order
            Models.Order order = new Models.Order();
            if (status)
            {
                order.UserId = HttpContext.Session.GetObject<User>("User").Id;
            }
            else
            {
                order.UserId = userOrder.Id;
            }
            order.DateOrder = DateTime.Now;
            order.DateShip = DateTime.Now.AddDays(3);
            order.IsPaid = false;
            order.IsDelete = false;
            order.IsDelivere = false;
            order.IsApproved = false;
            order.IsReceived = false;
            order.IsCancel = false;
            order.Offer = NumberDiscountPass;
            Models.Order o = _orderService.AddOrder(order);
            HttpContext.Session.SetInt32("OrderId", o.Id);
            //Add order detail
            List<ItemCart> listCart = GetCart();
            decimal sumtotal = 0;
            foreach (ItemCart item in listCart)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderId = order.Id;
                orderDetail.ProductId = item.ProductId;
                orderDetail.Quantity = item.Quantity;
                orderDetail.Price = item.Price;
                _orderDetailService.AddOrderDetail(orderDetail);
                sumtotal += orderDetail.Quantity * orderDetail.Price;
                if (HttpContext.Session.GetString("User") != null)
                {
                    //Remove Cart
                    _cartService.RemoveCart(item.ProductId, item.UserId);
                }
            }
            _orderService.UpdateTotal(order.Id, sumtotal);



            // Payment
            if (payment == "paypal")
            {
                return RedirectToAction("PaymentWithPaypal", "Payment");
            }
            else if (payment == "momo")
            {
                return RedirectToAction("PaymentWithMomo", "Payment");
            }


            SentMail("Đặt hàng thành công", user.Email, "khuongip564gb@gmail.com", "google..khuongip564gb", "<p style=\"font-size:20px\">Cảm ơn bạn đã đặt hàng<br/>Mã đơn hàng của bạn là: " + order.Id);



            HttpContext.Session.Remove("Code");
            HttpContext.Session.Remove("num");
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("OrderId");


            return RedirectToAction("Message", new { mess = "Đặt hàng thành công" });
        }

        [HttpGet]
        public ActionResult Message(string mess)
        {
            ViewBag.Message = mess;
            return View();
        }
        public void SentMail(string Title, string ToEmail, string FromEmail, string Password, string Content)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(ToEmail);
            mail.From = new MailAddress(ToEmail);
            mail.Subject = Title;
            mail.Body = Content;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(FromEmail, Password);
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
       
    }
}