using HelperLand.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HelperLand.ViewModel
{
    public class BookServiceViewModel
    {
        public List<AddressViewModel> address { get; set; }
        public PostalCodeViewModel postalCodeViewModel { get; set; }
        public ServiceSetupViewModel ServiceSetupViewModel { get; set; }
        public string streetname { get; set; }
        public int houseno { get; set; }
        public string cityname { get; set; }
        public string phoneno { get; set; }
        public string postalCode { get; set; }

        [Required]
        public bool checkPolicy { get; set; }

        public int addressId { get; set; }
        public int addressId2 { get; set; }
    }
}
