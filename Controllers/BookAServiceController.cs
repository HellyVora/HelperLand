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
        public static int check_mysetting = 0;
        public static int pass_success = 0;
        public static int address_change = 0;
        public static int dashbord = 0;
        public static int service_accepted = 0;
        public static int sp_service_cancel = 0;
        public static int sp_service_complete = 0;
        public static int after_rate = 0;
        public static int sp_cust_block = 0;
        public static int admin_service_request = 0;
        public static int do_active = 0;

        CustomerViewModel customerViewModel = new CustomerViewModel();
        
        ServiceProviderViewModel ServiceProviderViewModel = new ServiceProviderViewModel();
        AdminViewModel AdminView = new AdminViewModel();

        public BookAServiceController(HelperLandContext helperLandContext)
        {
            _helperLandContext = helperLandContext;
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
            return RedirectToAction("book_service", "BookAService");
        }

        public IActionResult Customer()
        {
            if (address_change != 0)
            {
                ViewBag.add_change = "true";
                address_change = 0;
            }
            if (check_mysetting != 0)
            {
                ViewBag.datasave = "true";
                check_mysetting = 0;
            }
            if (pass_success != 0)
            {
                ViewBag.showPassModal = "yes";
                pass_success = 0;
            }
            if (dashbord != 0)
            {
                ViewBag.showDash = "yes";
                dashbord = 0;
            }
            if (after_rate != 0)
            {
                ViewBag.after_rate = "yes";
                after_rate = 0;
            }
            if (ModelState.IsValid)
            {
                var userId = Int32.Parse(HttpContext.Session.GetString("UserId"));
                User req = _helperLandContext.Users.FirstOrDefault(x => x.UserId == userId);

                if (req.DateOfBirth != null)
                {
                    var DOB_str = (req.DateOfBirth).ToString();
                    var DOB_day = Int32.Parse(DOB_str.Substring(0, 2));
                    var DOB_month = Int32.Parse(DOB_str.Substring(3, 2));
                    var DOB_year = Int32.Parse(DOB_str.Substring(6, 4));
                    Debug.WriteLine("this is fetched " + DOB_str);

                    ViewBag.DOB_day = DOB_day;
                    ViewBag.DOB_month = DOB_month;
                    ViewBag.DOB_year = DOB_year;
                }

                ViewBag.fname = req.FirstName;
                ViewBag.lname = req.LastName;
                ViewBag.email = (req.Email).ToString();
                ViewBag.phone = req.Mobile;
                ViewBag.lang = req.LanguageId;
                ViewBag.fetched_ms_data = "yes";

                ms_get_address();
                get_dashboard_service_request();
                get_servicehistory_services();
                return View(customerViewModel);
            }
            else
            {
                ViewBag.fetched_ms_data = null;
            }

            return View();
        }

        [HttpPost]
        public IActionResult Customer(CustomerViewModel mySetting)
        {
            if (ModelState.IsValid)
            {
                var userId = Int32.Parse(HttpContext.Session.GetString("UserId"));
                var user = _helperLandContext.Users.Where(u => u.UserId == userId).FirstOrDefault();
                var DOB_day = mySetting.dob_day;
                var DOB_month = mySetting.dob_month;
                var DOB_year = mySetting.dob_year;
                var DOB_f = DOB_year + "-" + DOB_month + "-" + DOB_day;
                DateTime DOB_final = DateTime.Parse(DOB_f);
                TimeSpan time = new TimeSpan(0, 0, 0);
                DOB_final = DOB_final.Date + time;


                user.FirstName = mySetting.user.FirstName;
                user.LastName = mySetting.user.LastName;
                user.Email = mySetting.user.Email;
                user.Mobile = mySetting.user.Mobile;
                user.LanguageId = mySetting.user.LanguageId;
                user.DateOfBirth = DOB_final;
                _helperLandContext.SaveChanges();
            }


            check_mysetting = 1;
            return RedirectToAction("Customer");
        }


        public void ms_get_address()
        {
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));
            var addresses = (from uaddress in _helperLandContext.UserAddresses
                             where uaddress.UserId == userid
                             select new UserAddress()
                             {
                                 AddressId = uaddress.AddressId,
                                 AddressLine1 = uaddress.AddressLine1,
                                 City = uaddress.City,
                                 Mobile = uaddress.Mobile,
                                 PostalCode = uaddress.PostalCode
                             }).ToList();
            if (addresses.FirstOrDefault() != null)
            {
                customerViewModel.userAddresses = new List<UserAddress>();
                foreach (var add in addresses)
                {
                    customerViewModel.userAddresses.Add(add);
                }
            }
        }

        public void get_dashboard_service_request()
        {
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));

            var new_services = (from sr in _helperLandContext.ServiceRequests
                                from sre in _helperLandContext.ServiceRequestExtras
                                from sra in _helperLandContext.ServiceRequestAddresses
                                from mr in _helperLandContext.Ratings
                                where sr.UserId == userid && (sr.Status == 0 || sr.Status == 1) && sr.ServiceStartDate > DateTime.Now && sr.ServiceRequestId == sre.ServiceRequestId && sr.ServiceRequestId == sra.ServiceRequestId
                                        && sr.ServiceRequestId == mr.ServiceRequestId
                                select new DashboardViewModel()
                                {
                                    service_id = sr.ServiceId,
                                    service_date = sr.ServiceStartDate,
                                    duration = sr.ServiceHours + sr.ExtraHours,
                                    service_amount = sr.TotalCost,
                                    extra_service = sre.ServiceExtraId,
                                    service_address = sra.AddressLine1,
                                    phone = sra.Mobile,
                                    email = sra.Email,
                                    comment = sr.Comments,
                                    has_pet = sr.HasPets,
                                    sp_rating = _helperLandContext.Ratings.Where(x => x.RatingTo == (_helperLandContext.Ratings.Where(x => x.ServiceRequestId == sr.ServiceRequestId).FirstOrDefault().RatingTo)).Average(x => x.Ratings),
                                    sp_name = _helperLandContext.Users.Where(x => x.UserId == sr.ServiceProviderId).FirstOrDefault().FirstName + " "
                                            + _helperLandContext.Users.Where(x => x.UserId == sr.ServiceProviderId).FirstOrDefault().LastName
                                }).ToList();
            if (new_services.FirstOrDefault() != null)
            {
                customerViewModel.futureRequests = new List<DashboardViewModel>();
                foreach (var ser in new_services)
                {
                    customerViewModel.futureRequests.Add(ser);
                }
            }


        }

        public void get_servicehistory_services()
        {
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));

            var old_services = (from sr in _helperLandContext.ServiceRequests
                                from sre in _helperLandContext.ServiceRequestExtras
                                from sra in _helperLandContext.ServiceRequestAddresses
                                from mr in _helperLandContext.Ratings
                                where sr.UserId == userid && sr.Status != 1 && sr.Status != 0 &&
                                        sr.ServiceRequestId == sre.ServiceRequestId && sr.ServiceRequestId == sra.ServiceRequestId &&
                                        sr.ServiceRequestId == mr.ServiceRequestId


                                select new ServiceViewModel()
                                {
                                    service_id = sr.ServiceId,
                                    service_date = sr.ServiceStartDate,
                                    duration = sr.ServiceHours + sr.ExtraHours,
                                    service_amount = sr.TotalCost,
                                    status = sr.Status,
                                    sp_rating = _helperLandContext.Ratings.Where(x => x.RatingTo == (_helperLandContext.Ratings.Where(x => x.ServiceRequestId == sr.ServiceRequestId).FirstOrDefault().RatingTo)).Average(x => x.Ratings),
                                    sp_name = _helperLandContext.Users.Where(x => x.UserId == sr.ServiceProviderId).FirstOrDefault().FirstName +
                                            " " + _helperLandContext.Users.Where(x => x.UserId == sr.ServiceProviderId).FirstOrDefault().LastName
                                }).ToList();

            if (old_services.FirstOrDefault() != null)
            {
                customerViewModel.pastServices = new List<ServiceViewModel>();
                foreach (var ser in old_services)
                {
                    customerViewModel.pastServices.Add(ser);
                }
            }



        }


        [HttpPost]
        public IActionResult ChangePass(CustomerViewModel customer)
        {

            if (ModelState.IsValid)
            {
                var userId = Int32.Parse(HttpContext.Session.GetString("UserId"));
                var user = _helperLandContext.Users.Where(u => u.UserId == userId).FirstOrDefault();
                if (user.Password == customer.pwd)
                {
                    user.Password = customer.user.Password;
                    _helperLandContext.SaveChanges();
                    pass_success = 1;
                    HttpContext.Session.SetString("oldpass", "ok");
                }
                else
                {
                    pass_success = 0;
                    HttpContext.Session.SetString("oldpass", "notok");

                }
            }
            return RedirectToAction("Customer");
        }

        [HttpPost]
        public IActionResult add_new_address(CustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                var userId = Int32.Parse(HttpContext.Session.GetString("UserId"));
                var email = HttpContext.Session.GetString("Email");
                UserAddress userAddress = new UserAddress()
                {
                    UserId = userId,
                    AddressLine1 = customer.street + " " + customer.hno,
                    City = customer.city,
                    PostalCode = customer.pincode,
                    Mobile = customer.phone,
                    IsDefault = false,
                    IsDeleted = false,
                    Email = email
                };
                _helperLandContext.UserAddresses.Add(userAddress);
                _helperLandContext.SaveChanges();
            }
            address_change = 1;
            return RedirectToAction("Customer");
        }
        [HttpPost]
        public IActionResult edit_address(CustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                string addressline = customer.street + " " + customer.hno;

                var user_add = _helperLandContext.UserAddresses.Where(x => x.AddressId == customer.hidden_add_id).FirstOrDefault();

                user_add.AddressLine1 = addressline;
                user_add.City = customer.city;
                user_add.Mobile = customer.phone;
                user_add.PostalCode = customer.pincode;

                _helperLandContext.SaveChanges();

            }
            address_change = 1;
            return RedirectToAction("Customer");
        }


        [HttpPost]
        public IActionResult delete_address(CustomerViewModel customer)
        {


            var add = _helperLandContext.UserAddresses.Where(x => x.AddressId == customer.delete_add_id).FirstOrDefault();
            _helperLandContext.UserAddresses.Remove(add);
            _helperLandContext.SaveChanges();

            address_change = 1;
            return RedirectToAction("Customer");
        }

        public IActionResult dashboard()
        {
            dashbord = 1;
            return RedirectToAction("Customer");
        }

        [HttpPost]
        public IActionResult reschedule_service(CustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                int service_id = customer.hidden_service_id;

                var startdate = customer.rescheduled_date;
                DateTime start_date = DateTime.Parse(startdate);
                var starttime = customer.rescheduled_time;
                int s_hr = Int32.Parse(starttime.Substring(0, 2));
                int s_min = Int32.Parse(starttime.Substring(3, 2));
                TimeSpan servicestarttime = new TimeSpan(s_hr, s_min, 0);

                start_date = start_date.Date + servicestarttime;

                var servicerequest = _helperLandContext.ServiceRequests.Where(x => x.ServiceId == service_id).FirstOrDefault();
                servicerequest.ServiceStartDate = start_date;
                _helperLandContext.SaveChanges();
            }
            return RedirectToAction("dashboard");
        }

        [HttpPost]
        public IActionResult cancel_service(CustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                int service_id = customer.hidden_delete_service;
                var service = _helperLandContext.ServiceRequests.Where(x => x.ServiceId == service_id).FirstOrDefault();

                service.Status = 3;

                _helperLandContext.SaveChanges();
            }
            return RedirectToAction("dashboard");
        }

        [HttpPost]
        public IActionResult book_service_call()
        {
            return RedirectToAction("book_service");
        }

        [HttpPost]
        public IActionResult sp_rating(CustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                var ser_id = customer.rate_ser_id;
                var ser_req_id = _helperLandContext.ServiceRequests.Where(x => x.ServiceId == ser_id).FirstOrDefault().ServiceRequestId;
                var rating_ser = _helperLandContext.Ratings.Where(x => x.ServiceRequestId == ser_req_id).FirstOrDefault();
                rating_ser.OnTimeArrival = customer.on_time_arrival;
                rating_ser.Friendly = customer.freindly;
                rating_ser.QualityOfService = customer.qualit_of_service;
                rating_ser.Ratings = (customer.on_time_arrival + customer.freindly + customer.qualit_of_service) / 3;
                rating_ser.Comments = customer.feedback;
                _helperLandContext.SaveChanges();
                after_rate = 1;
            }

            return RedirectToAction("Customer");
        }

        public IActionResult ServiceProvider()
        {
            ViewBag.spmysetting = "hello";
            if (pass_success == 1)
            {
                ViewBag.showPassModal = "truw";
                pass_success = 0;
            }
            if (service_accepted == 1)
            {
                ViewBag.service_accepted = "true";
                service_accepted = 0;
            }
            if (sp_service_cancel == 1)
            {
                ViewBag.sp_cancel_service = "true";
                sp_service_cancel = 0;
            }
            if (sp_service_complete == 1)
            {
                ViewBag.sp_complete_service = "true";
                sp_service_complete = 0;
            }
            if (sp_cust_block == 1)
            {
                ViewBag.sp_cust_block = "true";
                sp_cust_block = 0;
            }
            if (do_active == 1)
            {
                ViewBag.doactive = "true";
                do_active = 0;
            }
            ViewBag.spMydetail = "true";
            var userId = Int32.Parse(HttpContext.Session.GetString("UserId"));
            User req = _helperLandContext.Users.FirstOrDefault(x => x.UserId == userId);
            UserAddress add = _helperLandContext.UserAddresses.FirstOrDefault(x => x.UserId == userId);
            ServiceProviderViewModel.Firsname = req.FirstName;
            ServiceProviderViewModel.Lastname = req.LastName;
            ServiceProviderViewModel.Email = req.Email;
            ServiceProviderViewModel.phone = req.Mobile;
            ServiceProviderViewModel.nationalityid = req.NationalityId;
            ServiceProviderViewModel.is_active = req.IsActive;
            ServiceProviderViewModel.avatar = req.UserProfilePicture;
            string[] address = add.AddressLine1.Split(' ');
            var house = address[address.Length - 1];
            address = address.Where(val => val != house).ToArray();
            string add_str = string.Join(" ", address);
            ServiceProviderViewModel.street = add_str;
            ServiceProviderViewModel.house = house;
            ServiceProviderViewModel.postal = add.PostalCode;
            ServiceProviderViewModel.city = add.City;
            if (req.DateOfBirth != null)
            {
                var DOB_str = (req.DateOfBirth).ToString();
                ServiceProviderViewModel.dob_day = Int32.Parse(DOB_str.Substring(0, 2));
                ServiceProviderViewModel.dob_month = Int32.Parse(DOB_str.Substring(3, 2));
                ServiceProviderViewModel.dob_year = Int32.Parse(DOB_str.Substring(6, 4));

            }
            ViewBag.sp_gender = req.Gender;

            get_sp_new_service_request();
            get_sp_upcoming_services();
            get_sp_service_history();
            get_sp_my_rating();
            get_sp_customers();
            return View(ServiceProviderViewModel);
        }

        [HttpPost]
        public IActionResult spMydetail(ServiceProviderViewModel spMySetting)
        {
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));
            var u = _helperLandContext.Users.Where(x => x.UserId == userid).FirstOrDefault();
            var ua = _helperLandContext.UserAddresses.Where(x => x.UserId == userid).FirstOrDefault();
            u.FirstName = spMySetting.Firsname;
            u.LastName = spMySetting.Lastname;
            u.Mobile = spMySetting.phone;
            u.NationalityId = spMySetting.nationalityid;
            var DOB_day = spMySetting.dob_day;
            var DOB_month = spMySetting.dob_month;
            var DOB_year = spMySetting.dob_year;
            var DOB_f = DOB_year + "-" + DOB_month + "-" + DOB_day;
            DateTime DOB_final = DateTime.Parse(DOB_f);
            TimeSpan time = new TimeSpan(0, 0, 0);
            DOB_final = DOB_final.Date + time;
            u.DateOfBirth = DOB_final;
            u.Gender = spMySetting.gender;
            u.UserProfilePicture = spMySetting.hidden_avatar.ToString();
            ua.AddressLine1 = spMySetting.street + " " + spMySetting.house;
            ua.PostalCode = spMySetting.postal;
            ua.City = spMySetting.city;
            u.ZipCode = spMySetting.postal;
            _helperLandContext.SaveChanges();
            return RedirectToAction("spMySetting");
        }

        [HttpPost]
        public IActionResult spChangepass(ServiceProviderViewModel mySetting)
        {

            if (ModelState.IsValid)
            {
                var userId = Int32.Parse(HttpContext.Session.GetString("UserId"));
                var user = _helperLandContext.Users.Where(u => u.UserId == userId).FirstOrDefault();
                if (user.Password == mySetting.pwd)
                {
                    user.Password = mySetting.new_pwd;
                    _helperLandContext.SaveChanges();
                    pass_success = 1;
                    HttpContext.Session.SetString("oldpass", "ok");
                }
                else
                {
                    pass_success = 0;
                    HttpContext.Session.SetString("oldpass", "notok");

                }
            }
            return RedirectToAction("spMySetting");
        }

        [HttpPost]
        public IActionResult accept_service(ServiceProviderViewModel spMySetting)
        {
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));
            bool isactive = _helperLandContext.Users.Where(x => x.UserId == userid).FirstOrDefault().IsActive;
            if (isactive)
            {
                var service = _helperLandContext.ServiceRequests.Where(x => x.ServiceId == spMySetting.hidden_nsr_ser_id);
                service.FirstOrDefault().Status = 1;
                service.FirstOrDefault().ServiceProviderId = userid;
                var myrating = _helperLandContext.Ratings.Where(x => x.ServiceRequestId == service.FirstOrDefault().ServiceRequestId);
                myrating.FirstOrDefault().RatingTo = userid;
                //var sp_avg_rate = _helperLandContext.Ratings.Where(x => x.RatingTo == userid).Average(x=>x.Ratings);
                //myrating.FirstOrDefault().Ratings = sp_avg_rate;
                _helperLandContext.SaveChanges();
                service_accepted = 1;
            }
            else
            {
                do_active = 1;
            }

            return RedirectToAction("spMySetting");
        }

        [HttpPost]
        public IActionResult sp_cancel_service(ServiceProviderViewModel spMySetting)
        {
            var service = _helperLandContext.ServiceRequests.Where(x => x.ServiceId == spMySetting.hidden_cancel_ser_id);
            if (service.FirstOrDefault().ServiceStartDate < DateTime.Now)
            {
                service.FirstOrDefault().Status = 3;
            }
            else
            {
                service.FirstOrDefault().ServiceProviderId = null;
                var myrating = _helperLandContext.Ratings.Where(x => x.ServiceRequestId == service.FirstOrDefault().ServiceRequestId);
                myrating.FirstOrDefault().RatingTo = 1;
                myrating.FirstOrDefault().Ratings = 0;
                service.FirstOrDefault().Status = 0;
            }

            _helperLandContext.SaveChanges();
            sp_service_cancel = 1;
            HttpContext.Session.SetString("cant_complete_ser", "false");
            return RedirectToAction("spMySetting");
        }

        [HttpPost]
        public IActionResult sp_complete_service(ServiceProviderViewModel spMySetting)
        {
            var ser = _helperLandContext.ServiceRequests.Where(x => x.ServiceId == spMySetting.hidden_complete_ser_id);
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));
            var check_blocked = _helperLandContext.FavoriteAndBlockeds.Where(x => x.UserId == userid).Where(x => x.TargetUserId == spMySetting.customer_id);
            if (check_blocked == null)
            {
                FavoriteAndBlocked favblock = new FavoriteAndBlocked()
                {
                    UserId = userid,
                    TargetUserId = spMySetting.customer_id,
                    IsFavorite = false,
                    IsBlocked = false
                };
                _helperLandContext.FavoriteAndBlockeds.Add(favblock);
                _helperLandContext.SaveChanges();
            }
            int duration_hr = 0;
            int duration_min = 0;
            DateTime end_time = ser.FirstOrDefault().ServiceStartDate;
            double duration = Convert.ToDouble(ser.FirstOrDefault().ServiceHours + ser.FirstOrDefault().ExtraHours);
            string dura_str = duration.ToString();
            bool dura_is_decimal = dura_str.Contains(".");
            string[] dura_arr = dura_str.Split(".");
            if (dura_is_decimal)
            {
                duration_hr = Int32.Parse(dura_arr[0]);



                if (Int32.Parse(dura_arr[1]) != 0)
                {
                    duration_min = 30;
                }
                else
                {
                    duration_min = 0;
                }
            }
            else
            {
                duration_hr = Int32.Parse(dura_str);
                duration_min = 0;

            }


            TimeSpan end_time_cal = new TimeSpan(duration_hr, duration_min, 0);

            end_time = end_time + end_time_cal;


            if (DateTime.Now > end_time)
            {
                ser.FirstOrDefault().Status = 2;
                sp_service_complete = 1;
                _helperLandContext.SaveChanges();
            }
            else
            {
                HttpContext.Session.SetString("cant_complete_ser", "true");
            }
            return RedirectToAction("ServiceProvider");
        }

        [HttpPost]
        public IActionResult unblock_cust(ServiceProviderViewModel spMySetting)
        {
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));
            var cust = _helperLandContext.FavoriteAndBlockeds.Where(x => x.UserId == userid && x.TargetUserId == spMySetting.customer_id).FirstOrDefault();
            cust.IsBlocked = false;
            _helperLandContext.SaveChanges();
            sp_cust_block = 1;
            return RedirectToAction("spMySetting");
        }

        [HttpPost]
        public IActionResult block_cust(ServiceProviderViewModel spMySetting)
        {
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));
            var cust = _helperLandContext.FavoriteAndBlockeds.Where(x => x.UserId == userid && x.TargetUserId == spMySetting.customer_id).FirstOrDefault();
            cust.IsBlocked = true;
            _helperLandContext.SaveChanges();
            sp_cust_block = 1;
            return RedirectToAction("spMySetting");
        }

        public IActionResult spNewServices()
        {
            service_accepted = 1;
            return RedirectToAction("spMySetting");
        }

        //========================== admin============================
        public IActionResult Admin()
        {
            if (admin_service_request == 1)
            {
                ViewBag.admin_service = "on";
                admin_service_request = 0;
            }
            get_admin_users();
            get_admin_service_requests();
            return View(AdminView);
        }

        [HttpPost]
        public IActionResult service_reschedule_admin(AdminViewModel admin)
        {
            int ser_id = admin.hidden_ser_id;
            var startdate = admin.c_ser_date;
            DateTime start_date = DateTime.Parse(startdate);
            var starttime = admin.c_ser_time;
            int s_hr = Int32.Parse(starttime.Substring(0, 2));
            int s_min = Int32.Parse(starttime.Substring(3, 2));
            TimeSpan servicestarttime = new TimeSpan(s_hr, s_min, 0);
            start_date = start_date.Date + servicestarttime;


            var service = _helperLandContext.ServiceRequests.Where(x => x.ServiceId == ser_id).FirstOrDefault();
            var ser_req_id = _helperLandContext.ServiceRequests.Where(x => x.ServiceId == ser_id).FirstOrDefault().ServiceRequestId;
            var service_address = _helperLandContext.ServiceRequestAddresses.Where(x => x.ServiceRequestId == ser_req_id).FirstOrDefault();


            service.ServiceStartDate = start_date;
            service_address.AddressLine1 = admin.c_street + " " + admin.c_hno;
            service_address.City = admin.c_city;
            service_address.PostalCode = admin.c_postal;
            _helperLandContext.SaveChanges();
            admin_service_request = 1;
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public IActionResult deactivate_user(AdminViewModel admin)
        {
            int u_id = admin.hidden_u_id;
            var user = _helperLandContext.Users.Where(x => x.UserId == u_id).FirstOrDefault();
            user.IsActive = false;
            _helperLandContext.SaveChanges();
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public IActionResult activate_user(AdminViewModel admin)
        {
            int u_id = admin.hidden_u_id;
            var user = _helperLandContext.Users.Where(x => x.UserId == u_id).FirstOrDefault();
            user.IsActive = true;
            _helperLandContext.SaveChanges();
            return RedirectToAction("Admin");
        }



        //-----------------------non action methods------------------------------------
        public bool email_exist(string email)
        {
            var isCheck = _helperLandContext.Users.Where(eMail => eMail.Email == email).FirstOrDefault();
            return isCheck != null;
        }

              //----------------------service provider methods ----------------------
        public void get_sp_new_service_request()
        {
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));
            if (ModelState.IsValid)
            {
                var user_zip = from zip in _helperLandContext.Users
                               where zip.UserId == userid
                               select new
                               {
                                   zip.ZipCode
                               };
                var user_Zipcode = user_zip.FirstOrDefault().ZipCode;
                //var new_services = (from sr in _helperLandContext.ServiceRequests
                //                    from sre in _helperLandContext.ServiceRequestExtras
                //                    from sra in _helperLandContext.ServiceRequestAddresses
                //                    from uid in _helperLandContext.Users
                //                    from favblock in _helperLandContext.FavoriteAndBlockeds
                //                    where sr.Status == 0 && sr.ZipCode == user_Zipcode 
                //                        && sr.ServiceRequestId == sre.ServiceRequestId 
                //                        && sr.ServiceRequestId == sra.ServiceRequestId 
                //                        && sr.UserId == uid.UserId 
                //                        && sr.ServiceStartDate > DateTime.Now
                //                        && sr.UserId == favblock.TargetUserId
                //                        && userid == favblock.UserId
                //                        && !(_helperLandContext.FavoriteAndBlockeds.Where(x=>x.UserId == userid && x.TargetUserId == sr.UserId).FirstOrDefault().IsBlocked)
                //                    select new NewServiceRequestViewModel()
                //                    {
                //                        service_id = sr.ServiceId,
                //                        service_date = sr.ServiceStartDate,
                //                        duration = sr.ServiceHours + sr.ExtraHours,
                //                        service_amount = sr.TotalCost,
                //                        extra_service = sre.ServiceExtraId,
                //                        service_address = sra.AddressLine1,
                //                        phone = sra.Mobile,
                //                        email = sra.Email,
                //                        comment = sr.Comments,
                //                        has_pet = sr.HasPets,
                //                        cust_name = uid.FirstName,
                //                        pincode = sra.PostalCode,
                //                        city = sra.City
                //                    }).ToList();
                var req = _helperLandContext.ServiceRequests.Where(x => x.ZipCode == user_Zipcode && x.ServiceStartDate > DateTime.Now && x.ServiceProviderId == null && x.Status == 0).ToList();
                if (req.Count() > 0)
                {
                    ServiceProviderViewModel.newServices = new List<NewServiceViewModel>();
                    foreach (var item in req)
                    {
                        var isBlocked = _helperLandContext.FavoriteAndBlockeds.FirstOrDefault(x => x.UserId == userid && x.TargetUserId == item.UserId && x.IsBlocked == true);

                        if (isBlocked == null)
                        {
                            NewServiceViewModel res = new NewServiceViewModel();
                            res.service_id = item.ServiceId;
                            res.service_date = item.ServiceStartDate;
                            res.duration = item.ServiceHours + item.ExtraHours;
                            res.service_amount = item.TotalCost;
                            res.comment = item.Comments;
                            res.has_pet = item.HasPets;

                            var sre = _helperLandContext.ServiceRequestExtras.FirstOrDefault(x => x.ServiceRequestId == item.ServiceRequestId);
                            res.extra_service = sre.ServiceExtraId;

                            var uid = _helperLandContext.Users.FirstOrDefault(x => x.UserId == item.UserId);
                            res.cust_name = uid.FirstName;


                            var sra = _helperLandContext.ServiceRequestAddresses.FirstOrDefault(x => x.ServiceRequestId == item.ServiceRequestId);
                            res.pincode = sra.PostalCode;
                            res.city = sra.City;
                            res.service_address = sra.AddressLine1;
                            res.phone = sra.Mobile;
                            res.email = sra.Email;
                            res.conflictId = null;
                            var check = _helperLandContext.ServiceRequests.Where(x => x.ServiceProviderId == userid && x.ServiceStartDate > DateTime.Now && x.Status == 1).ToList();
                            if (check.Count() > 0)
                            {
                                foreach (var i in check)
                                {
                                    var newSerStartDT = item.ServiceStartDate;
                                    var newSerEndDT = item.ServiceStartDate.AddHours((double)item.ServiceHours + (double)item.ExtraHours);
                                    var accSerStartDT = i.ServiceStartDate;
                                    var accSerEndDT = i.ServiceStartDate.AddHours((double)i.ServiceHours + (double)i.ExtraHours + 1);

                                    if (newSerStartDT >= accSerStartDT && newSerStartDT <= accSerEndDT)
                                    {
                                        res.conflictId = i.ServiceId;
                                        break;
                                    }
                                    else if (newSerEndDT >= accSerStartDT && newSerEndDT <= accSerEndDT)
                                    {
                                        res.conflictId = i.ServiceId;
                                        break;
                                    }
                                    else if (newSerStartDT < accSerStartDT && newSerEndDT > accSerEndDT)
                                    {
                                        res.conflictId = i.ServiceId;
                                        break;
                                    }
                                    else
                                    {
                                        res.conflictId = null;
                                    }
                                }
                            }
                            else
                            {
                                res.conflictId = null;
                            }

                            ServiceProviderViewModel.newServices.Add(res);
                        }
                    }
                }

            }
        }
        public void get_sp_upcoming_services()
        {
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));
            if (ModelState.IsValid)
            {
                var user_zip = from zip in _helperLandContext.Users
                               where zip.UserId == userid
                               select new
                               {
                                   zip.ZipCode
                               };
                var user_Zipcode = user_zip.FirstOrDefault().ZipCode;
                var up_services = (from sr in _helperLandContext.ServiceRequests
                                   from sre in _helperLandContext.ServiceRequestExtras
                                   from sra in _helperLandContext.ServiceRequestAddresses
                                   from uid in _helperLandContext.Users
                                   where sr.Status == 1 && sr.ZipCode == user_Zipcode && sr.ServiceRequestId == sre.ServiceRequestId && sr.ServiceRequestId == sra.ServiceRequestId && sr.UserId == uid.UserId
                                   select new UpcomingServicesViewModel()
                                   {
                                       cust_id = sr.UserId,
                                       service_id = sr.ServiceId,
                                       service_date = sr.ServiceStartDate,
                                       duration = sr.ServiceHours + sr.ExtraHours,
                                       service_amount = sr.TotalCost,
                                       extra_service = sre.ServiceExtraId,
                                       service_address = sra.AddressLine1,
                                       comment = sr.Comments,
                                       has_pet = sr.HasPets,
                                       cust_first_name = uid.FirstName,
                                       cust_last_name = uid.LastName,
                                       distance = sr.Distance,
                                       pincode = sra.PostalCode,
                                       city = sra.City
                                   }).ToList();
                if (up_services.FirstOrDefault() != null)
                {
                    ServiceProviderViewModel.upcomingServices = new List<UpcomingServicesViewModel>();
                    foreach (var ser in up_services)
                    {
                        ServiceProviderViewModel.upcomingServices.Add(ser);
                    }
                }
            }

        }
        public void get_sp_service_history()
        {
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));
            var complete_ser = from sr in _helperLandContext.ServiceRequests
                               from sra in _helperLandContext.ServiceRequestAddresses
                               where sr.ServiceProviderId == userid && sr.Status == 2 && sr.ServiceRequestId == sra.ServiceRequestId
                               select new ServiceHistoryViewModel()
                               {
                                   service_id = sr.ServiceId,
                                   cust_name = _helperLandContext.Users.Where(x => x.UserId == sr.UserId).FirstOrDefault().FirstName + " "
                                                + _helperLandContext.Users.Where(x => x.UserId == sr.UserId).FirstOrDefault().LastName,
                                   addressline1 = sra.AddressLine1,
                                   pincode = sra.PostalCode,
                                   city = sra.City,
                                   service_datetime = sr.ServiceStartDate,
                                   duration = sr.ServiceHours + sr.ExtraHours
                               };
            if (complete_ser.FirstOrDefault() != null)
            {
                ServiceProviderViewModel.spServiceHistories = new List<ServiceHistoryViewModel>();
                foreach (var data in complete_ser)
                {
                    ServiceProviderViewModel.spServiceHistories.Add(data);
                }
            }
        }
        public void get_sp_my_rating()
        {
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));
            var my_rating = from mr in _helperLandContext.Ratings
                            from sr in _helperLandContext.ServiceRequests

                            where sr.ServiceProviderId == userid && sr.Status == 2 && mr.RatingTo == userid && sr.ServiceRequestId == mr.ServiceRequestId
                            select new RatingViewModel()
                            {
                                service_id = sr.ServiceId,
                                service_datetime = sr.ServiceStartDate,
                                duration = sr.ServiceHours + sr.ExtraHours,
                                rating = (mr.OnTimeArrival + mr.QualityOfService + mr.Friendly) / 3,
                                cust_feedback = mr.Comments,
                                cust_name = _helperLandContext.Users.Where(x => x.UserId == sr.UserId).FirstOrDefault().FirstName + " "
                                                + _helperLandContext.Users.Where(x => x.UserId == sr.UserId).FirstOrDefault().LastName
                            };
            if (my_rating.FirstOrDefault() != null)
            {
                ServiceProviderViewModel.spMyratings = new List<RatingViewModel>();
                foreach (var rating in my_rating)
                {
                    ServiceProviderViewModel.spMyratings.Add(rating);
                }
            }

        }
        public void get_sp_customers()
        {
            var userid = Int32.Parse(HttpContext.Session.GetString("UserId"));
            var my_customers = (from favblock in _helperLandContext.FavoriteAndBlockeds
                                from u in _helperLandContext.Users
                                where favblock.UserId == userid && favblock.TargetUserId == u.UserId
                                select new BlockedCustomerViewModel()
                                {
                                    cust_id = favblock.TargetUserId,
                                    cust_name = _helperLandContext.Users.Where(x => x.UserId == favblock.TargetUserId).FirstOrDefault().FirstName + " " +
                                                 _helperLandContext.Users.Where(x => x.UserId == favblock.TargetUserId).FirstOrDefault().LastName,
                                    blocked = favblock.IsBlocked
                                }).ToList();
            if (my_customers.FirstOrDefault() != null)
            {
                ServiceProviderViewModel.spBlockCustomers = new List<BlockedCustomerViewModel>();
                foreach (var cust in my_customers)
                {
                    ServiceProviderViewModel.spBlockCustomers.Add(cust);
                }
            }
        }

        //-----------------------admin methods---------------------------------
        public void get_admin_users()
        {
            var u = _helperLandContext.Users;
            if (u.FirstOrDefault() != null)
            {
                AdminView.users = new List<User>();
                foreach (var data in u)
                {
                    AdminView.users.Add(data);
                }
            }
        }
        public void get_admin_service_requests()
        {
            var services = from sr in _helperLandContext.ServiceRequests
                           from sra in _helperLandContext.ServiceRequestAddresses
                           from mr in _helperLandContext.Ratings
                           where sr.ServiceRequestId == sra.ServiceRequestId && sr.ServiceRequestId == mr.ServiceRequestId
                           select new AdminServiceRequestViewModel()
                           {
                               ser_id = sr.ServiceId,
                               start_date = sr.ServiceStartDate,
                               duration = sr.ServiceHours + sr.ExtraHours,
                               cust_name = _helperLandContext.Users.Where(x => x.UserId == sr.UserId).FirstOrDefault().FirstName + " "
                                            + _helperLandContext.Users.Where(x => x.UserId == sr.UserId).FirstOrDefault().LastName,
                               addressline1 = sra.AddressLine1,
                               pincode = sra.PostalCode,
                               city = sra.City,
                               sp_id = sr.ServiceProviderId,
                               sp_name = _helperLandContext.Users.Where(x => x.UserId == sr.ServiceProviderId).FirstOrDefault().FirstName + " "
                                         + _helperLandContext.Users.Where(x => x.UserId == sr.ServiceProviderId).FirstOrDefault().LastName,
                               sp_rating = (mr.OnTimeArrival + mr.QualityOfService + mr.Friendly) / 3,
                               amount = sr.TotalCost,
                               status = sr.Status
                           };
            if (services.FirstOrDefault() != null)
            {

                AdminView.adminServiceRequests = new List<AdminServiceRequestViewModel>();
                foreach (var ser in services)
                {
                    AdminView.adminServiceRequests.Add(ser);
                }
            }
        }
    }
}


