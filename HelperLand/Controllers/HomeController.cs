using HelperLand.Models;
using HelperLand.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HelperLand.Data;
using HelperLand.Models;
using Microsoft.AspNetCore.Http;


namespace HelperLand.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HelperLandContext _helperLandContext=new HelperLandContext();


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //public HomeController(HelperLandContext helperLandContext)
        //{
        //    _helperLandContext = helperLandContext;
        //}

        public IActionResult Index()
        {
            User user = new User();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ViewResult Contactus() => View();

        public ViewResult Prices() => View();

        public ViewResult About() => View();

        public ViewResult Faq() => View();

        public ViewResult BecomeAPro()
        {
            return View();
        }
        public IActionResult SignupPage()
        {
            CustomerRegistrationViewModel customerRegistrationViewModel = new CustomerRegistrationViewModel();
            return View();
        }

        public bool email_check(string email)
        {
            var isCheck = _helperLandContext.Users.Where(eMail => eMail.Email == email).FirstOrDefault();
            return isCheck != null;
        }


        [HttpPost]

        public IActionResult SignupPage(CustomerRegistrationViewModel model
            )
        {
            var check = email_check(model.Email);
            if (check)
            {
                ModelState.AddModelError("Email", "Email already exist");
                return View();
            }
            else
            {
                User user = new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Mobile = model.Phonenumber,
                    Password = model.Password,

                    UserTypeId = 1,
                    IsRegisteredUser = true,
                    WorksWithPets = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = 0,
                    IsApproved = true,
                    IsActive = true,
                    IsDeleted = false
                };
                _helperLandContext.Users.Add(user);
                _helperLandContext.SaveChanges();
                ViewBag.successModal = string.Format("valid");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult BecomeAPro(CustomerRegistrationViewModel model)
        {
            var check = email_check(model.Email);
            if (check)
            {
                ModelState.AddModelError("Email", "Email already exist");
                return View();
            }
            else
            {
                User user = new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Mobile = model.Phonenumber,
                    Password = model.Password,

                    UserTypeId = 2,
                    IsRegisteredUser = true,
                    WorksWithPets = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = 0,
                    IsApproved = true,
                    IsActive = true,
                    IsDeleted = false
                };
                _helperLandContext.Users.Add(user);
                _helperLandContext.SaveChanges();
                ViewBag.successModal = string.Format("valid");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginViewModel loginViewModel, IFormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var details = (from userlist in _helperLandContext.Users
                               where userlist.Email == loginViewModel.Email && userlist.Password == loginViewModel.Password
                               select new
                               {
                                   userlist.UserId,
                                   userlist.FirstName,
                                   userlist.UserTypeId
                               }).ToList();
                if (details.FirstOrDefault() != null)
                {
                    HttpContext.Session.SetString("UserId",
                                                  details.FirstOrDefault().UserId.ToString());
                    HttpContext.Session.SetString("FirstName", details.FirstOrDefault().FirstName);
                    HttpContext.Session.SetString("UserTypeId", details.FirstOrDefault().UserTypeId.ToString());
                    if (loginViewModel.remember == true)
                    {
                        CookieOptions options = new CookieOptions();
                        options.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Append("Email", fc["Email"], options);
                        Response.Cookies.Append("Password", fc["Password"], options);
                    }
                    else
                    {
                        Response.Cookies.Delete("Email");
                        Response.Cookies.Delete("Password");
                    }
                    HttpContext.Session.SetString("loggedIn", "yes");
                    HttpContext.Session.SetString("UserName", details.FirstOrDefault().FirstName);

                 //   return View("Index");
                 return RedirectToAction("ContactUs");


                }
                else
                {

                    ModelState.AddModelError("", "Invalid Credentials");
                    ViewBag.modal = string.Format("invalid username or password");
                    ViewBag.logged = null;
                    return RedirectToAction("Index");
                }
            }
            return View(loginViewModel);

        }
            
        
    }
}
