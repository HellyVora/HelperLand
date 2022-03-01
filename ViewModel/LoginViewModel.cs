using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HelperLand.Models;

namespace HelperLand.ViewModel
{
    public class LoginViewModel
    {
        public User user { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool remember { get; set; }
    }
}
