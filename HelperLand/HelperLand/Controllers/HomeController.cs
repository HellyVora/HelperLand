using HelperLand.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HelperLand.Data;
using HelperLand.Models;

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
            User user = new User();
            return View(user);
        }
        public IActionResult SignupPage()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]

        public IActionResult SignupPage(User user)
        {
            if(_helperLandContext.Users.Where(x => x.Email == user.Email).Count()==0 && _helperLandContext.Users.Where(x => x.Mobile == user.Mobile).Count() == 0)
            {
                user.UserTypeId = 1;
                user.CreatedDate = DateTime.Now;
                user.ModifiedDate = DateTime.Now;
                user.IsRegisteredUser = true;
                user.ModifiedBy = 123;

                _helperLandContext.Users.Add(user);
                _helperLandContext.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.message = "User already exists";
            }
            return View();
        }
        [HttpPost]
        public IActionResult BecomeAPro(User user)
        {
            if (_helperLandContext.Users.Where(x => x.Email == user.Email).Count() == 0 && _helperLandContext.Users.Where(x => x.Mobile == user.Mobile).Count() == 0)
            {
                user.UserTypeId = 2;
                user.CreatedDate = DateTime.Now;
                user.ModifiedDate = DateTime.Now;
                user.IsRegisteredUser = true;
                user.ModifiedBy = 123;

                _helperLandContext.Users.Add(user);
                _helperLandContext.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.message = "User already exists";
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(User user)
        {
            if (_helperLandContext.Users.Where(x => x.Email == user.Email).Count() == 0)
            {
                if ( _helperLandContext.Users.Where(x => x.Password == user.Password).Count() == 0)
                         ViewBag.message = "enter correct  password";
            }
            else
            {
                ViewBag.message = "Login Successfull";
                return RedirectToAction("ContactUs");
            }
                return View();
            

        }
            
        
    }
}
