using HelperLand.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HelperLand.ViewModel
{
    public class CustomerViewModel
    {
        public User user { get; set; }
        public List<UserAddress> userAddresses { get; set; }
        public int dob_day { get; set; }
        public int dob_month { get; set; }
        public int dob_year { get; set; }


        public string pwd { get; set; }

        public string street { get; set; }
        public int hno { get; set; }
        public string phone { get; set; }
        public string pincode { get; set; }
        public string city { get; set; }

        public int hidden_add_id { get; set; }
        public int delete_add_id { get; set; }


        // ---------------------------for dashboard --------------------------
        public List<DashboardViewModel> futureRequests { get; set; }

        public string rescheduled_date { get; set; }
        public string rescheduled_time { get; set; }
        public int hidden_service_id { get; set; }
        public int hidden_delete_service { get; set; }
        //----------------------------for service history ----------------------------
        public List<ServiceViewModel> pastServices { get; set; }
        //----------------------------for ratings -------------------------------
        public decimal on_time_arrival { get; set; }
        public decimal freindly { get; set; }
        public decimal qualit_of_service { get; set; }
        public string feedback { get; set; }
        public int rate_ser_id { get; set; }
    }
}

