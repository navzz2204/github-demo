using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToyStoreOnlineWeb.Models;
using ToyStoreOnlineWeb.Service;
using ToyStoreOnlineWeb.Infrastructure;

namespace ToyStoreOnlineWeb.Controllers
{
    public class UserController : Controller
    {
        private IFileStorageService _fileStorageService;
        IUserService _userService;
        IOrderService _orderService;
        IOrderDetailService _orderDetailService;
        IProductService _productService;
        IRatingService _ratingService;

        ICartService _cartService;
        IUsersSpinService _usersSpinService;
        public UserController(IFileStorageService fileStorageService, IUserService userService, IOrderService orderService, IOrderDetailService orderDetailService, IProductService productService, IRatingService ratingService, ICartService cartService, IUsersSpinService usersSpinService)
        {
            _fileStorageService = fileStorageService;
            _userService = userService;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _productService = productService;
            _ratingService = ratingService;

            _cartService = cartService;
            _usersSpinService = usersSpinService;
        }
        [HttpGet]
        public ActionResult Index()
        {
            var user = HttpContext.Session.GetObject<User>("User");
            return View(user);
        }
        [HttpGet]
        public ActionResult EditName(int id)
        {
            var member = _userService.GetByID(id);
            //Check null
            if (member != null)
            {
                //Return view
                return Json(new
                {
                    ID = member.Id,
                    FullName = member.FullName,
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
        public ActionResult EditName(int ID, string FullName)
        {
            User user = _userService.GetByID(ID);
            user.FullName = FullName;
            _userService.Update(user);
            IEnumerable<User> users = _userService.GetList();
            foreach (User item in users)
            {
                if (item.PhoneNumber == user.PhoneNumber)
                {
                    item.FullName = user.FullName;
                    _userService.Update(item);
                }
            }
            HttpContext.Session.SetObject("User", user);
            TempData["EditName"] = "Sucess";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult EditAddress(int id)
        {
            var member = _userService.GetByID(id);
            //Check null
            if (member != null)
            {
                //Return view
                return Json(new
                {
                    ID = member.Id,
                    Address = member.Address,
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
        public ActionResult EditAddress(int ID, string Address)
        {
            User user = _userService.GetByID(ID);
            user.Address = Address;
            _userService.Update(user);
            HttpContext.Session.SetObject("Member", user);
            TempData["EditAddress"] = "Sucess";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> EditAvatar(IFormFile Avatar)
        {
            if (Avatar != null)
            {
                //Get file name
                var fileName = await _fileStorageService.SaveFileAsync(Avatar);


                // Get the current user from session
                var user = HttpContext.Session.GetObject<User>("User"); // Make sure to have the GetObject extension method
                if (user != null)
                {
                    // Update the user with the new avatar filename
                    User userUpdate = _userService.GetByID(user.Id);
                    userUpdate.Avatar = fileName;
                    _userService.Update(userUpdate);

                    // Update the user in session
                    HttpContext.Session.SetObject("User", userUpdate);

                    // Set a success message in TempData
                    TempData["EditAvatar"] = "Success";
                }
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult ConfirmEmail(int ID)
        {
            if (_userService.GetByID(ID).EmailConfirmed)
            {
                ViewBag.Message = "EmailConfirmed";
                return View();
            }
            string strString = "abcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            int randomCharIndex = 0;
            char randomChar;
            string captcha = "";
            for (int i = 0; i < 10; i++)
            {
                randomCharIndex = random.Next(0, strString.Length);
                randomChar = strString[randomCharIndex];
                captcha += Convert.ToString(randomChar);
            }
            var request = HttpContext.Request;
            string urlBase = $"{request.Scheme}://{request.Host}{Url.Content("~")}";
            ViewBag.Id = ID;
            string email = _userService.GetByID(ID).Email;
            ViewBag.Email = "Mã xác minh đã được gửi đến: " + email;
            _userService.UpdateCapcha(ID, captcha);
            //  SentMail("Mã xác minh tài khoản ToyStore", email, "Vendetta22x4@gmail.com", "google..khuongip564gb", "<p>Mã xác minh của bạn: " + captcha + "<br/>Hoặc xác minh nhanh bằng cách click vào link: " + urlBase + "/User/ConfirmEmailLink/" + ID + "?Capcha=" + captcha + "</p>");
            return View();
        }
        [HttpGet]
        public ActionResult ConfirmEmailLink(int ID, string capcha)
        {
            User user = _userService.CheckCapcha(ID, capcha);
            if (user != null)
            {
                ViewBag.Message = "EmailConfirmed";
                //Gift

                UsersSpin usersSpin = new UsersSpin();
                usersSpin.NumberOfSpins = 0;
                usersSpin.UserId = user.Id;
                _usersSpinService.Add(usersSpin);
                return View();
            }
            ViewBag.Message = "Mã xác minh tài khoản không đúng";
            ViewBag.ID = ID;
            return View(new { ID = ID });
        }
        [HttpPost]
        public ActionResult ConfirmEmail(int ID, string capcha)
        {
            User user = _userService.CheckCapcha(ID, capcha);
            if (user != null)
            {
                ViewBag.Message = "EmailConfirmed";

                UsersSpin usersSpin = new UsersSpin();
                usersSpin.NumberOfSpins = 0;
                usersSpin.UserId = user.Id;
                _usersSpinService.Add(usersSpin);
                return View();
            }
            ViewBag.Message = "Mã xác minh tài khoản không đúng";
            ViewBag.Id = ID;
            return View(new { ID = ID });
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
        public ActionResult Error(int CustomerID)
        {
            ViewBag.ID = CustomerID;
            return View();
        }
        [HttpGet]
        public ActionResult CheckoutOrder()
        {
            var userSession = HttpContext.Session.GetObject<User>("User");
            if (userSession != null)
            {
                string Phone = _userService.GetByID(userSession.Id).PhoneNumber;
                User user = _userService.GetList().FirstOrDefault(x => x.PhoneNumber == Phone);
                if (user != null)
                {
                    var orders = _orderService.GetAll().Where(x => x.User.PhoneNumber == user.PhoneNumber);
                    ViewBag.ProductRating = _ratingService.GetListAllRating().Where(x => x.UserId == user.Id);
                    return View(orders);
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult OrderDetail(int ID)
        {
            var user = HttpContext.Session.GetObject<User>("User");
            if (user == null)
            {
                return View();
            }
            if (ID == null)
            {
                return null;
            }
            Order order = _orderService.GetByID(ID);
            IEnumerable<OrderDetail> orderDetails = _orderDetailService.GetByOrderID(ID);
            if (orderDetails == null)
            {
                return null;
            }
            ViewBag.OrderID = ID;
            if (order.IsApproved)
            {
                ViewBag.Approved = "Approved";
            }
            if (order.IsDelivere)
            {
                ViewBag.Delivere = "Delivere";
            }
            if (order.IsReceived)
            {
                ViewBag.Received = "Received";
            }
            ViewBag.Total = order.Total;
            return View(orderDetails);
        }
        public ActionResult GetDataProduct(int ID)
        {
            Product product = _productService.GetByID(ID);
            return Json(new
            {
                ID = product.Id,
                Name = product.Name,
                Image = product.Image1,
                status = true
            });
        }
        public ActionResult Received(int OrderID)
        {
            var user = HttpContext.Session.GetObject<User>("User");
            //Update AmountPurchased for member
            if (user != null)
            {
                _orderService.Received(OrderID);
                _userService.UpdateAmountPurchased(user.Id, _orderService.GetByID(OrderID).Total.Value);
                //Add once spin
                _usersSpinService.AddOnceSpin(user.Id);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("OrderDetail", new { ID = OrderID });
        }
        public JsonResult Cancel(int ID)
        {
            Order order = _orderService.GetByID(ID);
            order.IsCancel = true;
            _orderService.Update(order);
            return Json(new
            {
                status = true
            });
        }
        [HttpGet]
        public ActionResult DeleteAccount()
        {
            var user = HttpContext.Session.GetObject<User>("User");
            return View(user);
        }
        [HttpPost]
        public JsonResult DeleteAccount(string Password)
        {
            var user = HttpContext.Session.GetObject<User>("User");
            User userCheck = _userService.CheckLogin(user.Email, Password);
            if (userCheck != null)
            {
                _userService.Block(userCheck);
                _cartService.RemoveCartDeleteAccount(user.Id);
                TempData["DeleteAccount"] = "Success";
                HttpContext.Session.Remove("User");
                HttpContext.Session.Remove("Cart");
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                TempData["IncorretPassword"] = "true";
            }
            return Json(new
            {
                status = false
            });
        }
        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string CurrentPassword, string NewPassword)
        {
            var user = HttpContext.Session.GetObject<User>("User");
            User userCheck = _userService.CheckLogin(user.Email, CurrentPassword);
            if (userCheck != null)
            {
                _userService.ResetPassword(userCheck.Id, NewPassword);
                TempData["ResetPassword"] = "Success";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Mật khẩu hiện tại không đúng!";
            }
            return RedirectToAction("Index");
        }

        public JsonResult GetOrderJson()
        {
            var list = _orderService.GetAll().Where(x => x.IsApproved == false).OrderByDescending(x => x.DateOrder).Take(5).Select(x => new { ID = x.Id, CustomerName = x.User.FullName, DateOrder = (DateTime.Now - x.DateOrder).Minutes });
            return Json(list);
        }
    }
}