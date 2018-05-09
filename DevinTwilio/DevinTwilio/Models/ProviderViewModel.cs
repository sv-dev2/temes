using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DevinTwilio.Models
{
    public class ProviderViewModel
    {
        public int ProviderId { get; set; }

        [Required]
        [Display(Name = "Provider Name")]
        public string ProviderName { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }
    }
}