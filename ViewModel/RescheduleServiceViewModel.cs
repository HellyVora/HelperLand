using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HelperLand.ViewModel
{
    public class RescheduleServiceViewModel
    {

        [Required]
        public int ServiceId { get; set; }
        [Required]
        public string NewServiceDate { get; set; } = null!;
        [Required]
        public string NewServicetime { get; set; } = null!;
    }
}
