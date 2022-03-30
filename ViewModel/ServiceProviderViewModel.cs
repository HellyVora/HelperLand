using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelperLand.ViewModel
{
    public class ServiceProviderViewModel
    {

        public List<NewServiceViewModel> newServices { get; set; }
        public List<UpcomingServicesViewModel> upcomingServices { set; get; }
        public List<ServiceHistoryViewModel> spServiceHistories { get; set; }
        public List<RatingViewModel> spMyratings { get; set; }
        public List<BlockedCustomerViewModel> spBlockCustomers { get; set; }
        public int hidden_nsr_ser_id { get; set; }
        public int hidden_complete_ser_id { get; set; }
        public int hidden_cancel_ser_id { get; set; }
        public int customer_id { get; set; }
        //--------------------for sp detail screen----------------------
        public bool is_active { get; set; }
        public string Firsname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string phone { get; set; }
        public int? nationalityid { get; set; }
        public int dob_day { get; set; }
        public int dob_month { get; set; }
        public int dob_year { get; set; }
        public int? gender { get; set; }
        public string avatar { get; set; }

        public string street { get; set; }
        public string house { get; set; }
        public string postal { get; set; }
        public string city { get; set; }
        public string pwd { get; set; }
        public int hidden_avatar { get; set; }
        public string new_pwd { get; set; }
    }
}
