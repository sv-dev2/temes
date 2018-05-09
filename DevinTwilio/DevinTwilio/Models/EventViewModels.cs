using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DevinTwilio.Models
{
    public class EventViewModels
    {
        public int Id { get; set;}

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string EventName { get; set; } // it's belong to appointment


        [Display(Name = "Event Type")]
        public string TriggerTypeName { get; set; }


        [Required(ErrorMessage = "Event Type is required")]
        [Display(Name = "Event Type")]
        public int? TriggerTypeId { get; set; }
        public List<TriggerType> TriggerTypeList { get; set; }

        [Display(Name = "Day")]
        //[Required(ErrorMessage = "Day is required")]
        public string TriggerEvent_Day { get; set; } // it's belong to enrollment

        [Display(Name = "Date Time")]
        public Nullable<System.DateTime> TriggerEvent_DateTime { get; set; }

        public string StrTriggerEvent_DateTime { get; set; }

    }
}