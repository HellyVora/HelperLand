using HelperLand.Data;
using HelperLand.Models;
using HelperLand.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HelperLand.Controllers
{
    public class BookAServiceController : Controller
    {
        private readonly HelperLandContext _helperLandContext;



        public BookAServiceController(HelperLandContext helperlandContext)
        {
            _helperLandContext = helperlandContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PostalCode(BookServiceViewModel bookServiceViewModel)
        {

            var spdetails = (from service in _helperLandContext.Users
                             where service.UserTypeId == 2 && service.ZipCode == bookServiceViewModel.postalCodeViewModel.zipcode
                             select new
                             {
                                 service.UserId,
                                 service.FirstName,
                                 service.LastName
                             }).ToList();
            if (spdetails.FirstOrDefault() != null)
            {
                HttpContext.Session.SetString("again_called", "spfound");
                HttpContext.Session.SetString("zipcode", bookServiceViewModel.postalCodeViewModel.zipcode);
                ViewBag.message="zipcode found";
                return RedirectToAction("Index", "BookAService");
            }
            else
            {
                HttpContext.Session.SetString("again_called", "temp");
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
