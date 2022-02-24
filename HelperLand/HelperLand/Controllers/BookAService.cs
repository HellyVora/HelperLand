using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelperLand.Models;
using HelperLand.Data;
using HelperLand.Controllers;

namespace HelperLand.Controllers
{
    public class BookAService : Controller
    {
        private readonly HelperLandContext _helperLandContext = new HelperLandContext();
        public IActionResult Index()
        {
            Zipcode zip = new Zipcode();
            return View();
        }
    }
}
