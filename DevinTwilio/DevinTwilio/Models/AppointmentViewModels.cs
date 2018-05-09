using DevinTwilio.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DevinTwilio.Models
{
    public class AppointmentViewModels
    {
        [Key]
        public int AppointmentID { get; set; }

        [Required]
        public int? UserId { get; set; }

        public string UserName { get; set; }

        [Required]
        public int? ProviderId { get; set; }

        [Display(Name = "Provider Name")]
        public string ProviderName { get; set; }

        [Required(ErrorMessage = "Appointment typee is required")]
        //[ForeignKey("AppointmentType")]
        public int Appointment_Trigger_EventId { get; set; }


        [Display(Name = "Type")]
        public string TypeName { get; set; }

        [Display(Name = "Event")]
        public string EventName { get; set; }

        public string Description { get; set; }

        [Required]

        [Display(Name = "Apointment Date")]
        public DateTime? StartTime { get; set; }

        public string StrStartTime { get; set; }

        [Display(Name = "Enrollment Date")]
        public string EnrollmentDate { get; set; }

        public string EndTime { get; set; }

        public string TriggerEventDay { get; set; }

        //[Required]
        //[ForeignKey("AppointmentType")]
        public int AppointmentTypeId { get; set; }

        public string AppointmentTypeName { get; set; }

        public List<UserViewModels> UsesList { get; set; }

        public List<DAL.TriggerEvent> AppointmentEventList { get; set; }

        public List<ProviderViewModel> ProviderList { get; set; }
        // public virtual TriggerType TriggerType { get; set; }
    }
}