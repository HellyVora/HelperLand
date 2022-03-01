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
        BookServiceViewModel userAddresses = new BookServiceViewModel();

        public static int cnt = 0;
       

        public BookAServiceController(HelperLandContext helperlandContext)
        {
            _helperLandContext = helperlandContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult book_service()
        {

            string uid = HttpContext.Session.GetString("UserId");
            if (uid != null)
            {
                string uname = HttpContext.Session.GetString("FirstName");
                ViewBag.Uname = uname;
                ViewBag.login_check = String.Format("loggedin");
                if (cnt != 0)
                {
                    if (HttpContext.Session.GetString("again_called") != "spfound")
                    {
                        HttpContext.Session.SetString("ss_step_2", "notset");
                        ViewBag.foundsp = string.Format("spnotfound");
                        string temp_var = ViewBag.foundsp;
                        Debug.WriteLine("this is viewbag foundsp" + temp_var);
                    }
                    else
                    {
                        ViewBag.foundsp = null;
                        HttpContext.Session.SetString("ss_step_2", "notset");

                    }

                }
                cnt = 1;
                int address_fetch_cnt = getAddress();

                HttpContext.Session.SetInt32("address_fetch_cnt", address_fetch_cnt);

                return View(userAddresses);
            }
            else
            {
                ViewBag.login_before_service = string.Format("please login first");
                return RedirectToAction("Index_Login", "Home");
            }

        }
        public int getAddress()
        {

            Debug.WriteLine("this methd is called");
            HttpContext.Session.SetString("getaddress", "set");
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));
            var addresses = (from uaddress in _helperLandContext.UserAddresses
                             where uaddress.UserId == userid
                             select new AddressViewModel()
                             {
                                 id = uaddress.AddressId,
                                 addressline1 = uaddress.AddressLine1,
                                 city = uaddress.City,
                                 phonenumber = uaddress.Mobile,
                                 postalcode = uaddress.PostalCode
                             }).ToList();

            var last_add_id = (from t in _helperLandContext.UserAddresses
                               where t.UserId == userid
                               orderby t.AddressId
                               select t.AddressId).Last();

            if (addresses.FirstOrDefault() != null)
            {

                userAddresses.address = new List<AddressViewModel>();
                foreach (var add in addresses)
                {
                    userAddresses.address.Add(add);

                }
            }

            return last_add_id;
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
                return RedirectToAction("book_service", "BookAService");
                // return Redirect(Url.Action("book_service", "BookAService") + "#tabContent2");
                //return View();
            }
            else
            {
                HttpContext.Session.SetString("again_called", "temp");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public IActionResult addServiceInfo(BookServiceViewModel bookServiceViewModel)
        {



            var startdate_str = bookServiceViewModel.ServiceSetupViewModel.servicestartdate;
            Debug.WriteLine(" this is startdate_str " + startdate_str);
            DateTime startdate = DateTime.Parse(startdate_str);
            var starttime = bookServiceViewModel.ServiceSetupViewModel.servicestrarttime;
            int s_hr = Int32.Parse(starttime.Substring(0, 2));
            int s_min = Int32.Parse(starttime.Substring(3, 2));
            TimeSpan servicestarttime = new TimeSpan(s_hr, s_min, 0);

            startdate = startdate.Date + servicestarttime;

            string userid = HttpContext.Session.GetString("UserId");
            string zip = HttpContext.Session.GetString("zipcode");
            float hours = bookServiceViewModel.ServiceSetupViewModel.servicehours;
            bool extraser1 = bookServiceViewModel.ServiceSetupViewModel.extraSer1;
            bool extraser2 = bookServiceViewModel.ServiceSetupViewModel.extraSer2;
            bool extraser3 = bookServiceViewModel.ServiceSetupViewModel.extraSer3;
            bool extraser4 = bookServiceViewModel.ServiceSetupViewModel.extraSer4;
            bool extraser5 = bookServiceViewModel.ServiceSetupViewModel.extraSer5;
            bool haspet = bookServiceViewModel.ServiceSetupViewModel.haspets;
            var getextraservice = extraser1 + "" + extraser2 + "" + extraser3 + "" + extraser4 + "" + extraser5;

            var trueto1 = getextraservice.Replace("True", "1");
            var falseto0 = trueto1.Replace("False", "0");
            int extraserInt = Int32.Parse(falseto0);

            float subtotal = hours * 20;
            double extra_hr = 0.0;
            if (extraser1) { subtotal += 10; extra_hr += 0.5; }
            if (extraser2) { subtotal += 10; extra_hr += 0.5; }
            if (extraser3) { subtotal += 10; extra_hr += 0.5; }
            if (extraser4) { subtotal += 10; extra_hr += 0.5; }
            if (extraser5) { subtotal += 10; extra_hr += 0.5; }
            decimal total = new decimal(subtotal);

            Debug.WriteLine("this is service start time " + startdate);
            ServiceRequest service = new ServiceRequest()
            {
                UserId = Int32.Parse(userid),
                ServiceId = 1,
                ZipCode = zip,
                ServiceStartDate = startdate,
                ServiceHourlyRate = 20,
                ServiceHours = hours,
                ExtraHours = extra_hr,
                SubTotal = total,
                TotalCost = total,
                PaymentDue = false,
                HasPets = haspet,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Distance = 10
            };
            _helperLandContext.ServiceRequests.Add(service);
            _helperLandContext.SaveChanges();

            //var getservicerequestid = _helperLandContext.ServiceRequests.Where(x => x.UserId == Int32.Parse(userid)).ToList();

            var getservicerequestid = (from temp in _helperLandContext.ServiceRequests
                                       where temp.UserId == Int32.Parse(userid)
                                       orderby temp.ServiceRequestId
                                       select temp.ServiceRequestId).Last();

            ServiceRequestExtra service1 = new ServiceRequestExtra()
            {
                ServiceRequestId = getservicerequestid,
                ServiceExtraId = extraserInt
            };
            _helperLandContext.ServiceRequestExtras.Add(service1);
            _helperLandContext.SaveChanges();

            var u_email = _helperLandContext.Users.Where(id => id.UserId == Int32.Parse(userid)).FirstOrDefault();

            var addressid = bookServiceViewModel.addressId;
            var addressid2 = bookServiceViewModel.addressId2;

            
            if (addressid2 != 0)
            {
                ServiceRequestAddress serviceRequestAddress = new ServiceRequestAddress()
                {
                    ServiceRequestId = getservicerequestid,
                    AddressLine1 = bookServiceViewModel.streetname + " " + bookServiceViewModel.houseno,
                    City = bookServiceViewModel.cityname,
                    PostalCode = bookServiceViewModel.postalCode,
                    Mobile = bookServiceViewModel.phoneno,
                    Email = u_email.Email

                };
                _helperLandContext.ServiceRequestAddresses.Add(serviceRequestAddress);
                _helperLandContext.SaveChanges();
            }
            else
            {
                var address = (from uaddress in _helperLandContext.UserAddresses
                               where uaddress.Email == u_email.Email
                               select new
                               {

                                   addressline1 = uaddress.AddressLine1,

                                   city = uaddress.City,
                                   phonenumber = uaddress.Mobile,

                                   postalcode = uaddress.PostalCode
                               });



                ServiceRequestAddress serviceRequestAddress = new ServiceRequestAddress()
                {
                    ServiceRequestId = getservicerequestid,
                    AddressLine1 = address.FirstOrDefault().addressline1,
                    City = address.FirstOrDefault().city,
                    PostalCode = address.FirstOrDefault().postalcode,
                    Mobile = address.FirstOrDefault().phonenumber,
                    Email = u_email.Email

                };
                _helperLandContext.ServiceRequestAddresses.Add(serviceRequestAddress);
                _helperLandContext.SaveChanges();
            }


            HttpContext.Session.SetString("showBookSuccess", "yes");

            HttpContext.Session.SetInt32("serviceRequestID", getservicerequestid);
            return RedirectToAction("Index","Home");
        }
    }
}
