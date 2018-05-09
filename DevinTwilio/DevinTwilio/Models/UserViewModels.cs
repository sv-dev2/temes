using DevinTwilio.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DevinTwilio.Models
{
    public class UserViewModels
    {
        public int User_ID { get; set; }
        [Required]
        [Display(Name = "User Name")]
        public string User_Name { get; set; }
      //  [Required]
        public string Password { get; set; }

      //  [Display(Name = "Confirm Password")]
       // [Compare("Password")]
    //    public string ConfirmPassword { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }
        public Nullable<int> Role_Id { get; set; }
        public string User_PhoneNumber { get; set; }

        [Display(Name = "Mobile Number")]
        [Required]
        // [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string MobileNumber { get; set; }
        public bool RememberMe { get; set; }
        public string RoleName { get; set; }

        [Display(Name = "Appointment Date")]
        public DateTime? AppointmentDate { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Enrollment Date")]

        public string strEnrollmentDate { get; set; }

        [Display(Name = "Enrollment Date")]
        public DateTime? EnrollmentDate { get; set; }

        public List<DAL.Role> Roles { get; set; }

        public string LastMessage { get; set; }

        [Display(Name = "Next Appointment")]
        public DateTime? NextAppointment { get; set; }

        public DateTime? LastLoggedIn { get; set; }

        [Key]
        public DateTime? CreatedDate { get; set; }

        public int MessageID { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }
        public List<UserViewModels> UsesList { get; set; }
        public string NextMessageEnrollment { get; set; }
        public string NextMessageAppointment { get; set; }

        [Display(Name = "Sent Date")]
        public DateTime? SentDate { get; set; }

        [Display(Name = "Response Date")]
        public DateTime? ResponseDate { get; set; }

        [Display(Name = "Patient Status")]
        public string UserStatus { get; set; }

        public List<UserStatu> UserStatusList { get; set; }

        public int? StatusId { get; set; }

        public string ActionToDo { get; set; }
    }

    public class AppointmentDetail
    {
        public int User_ID { get; set; }      
        public string User_Name { get; set; }   
        public DateTime? AppointmentDate { get; set; }      
        public DateTime? NextAppointment { get; set; }    

        public int MessageID { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }

        public int? TriggerEventId { get; set; }

        public string Time { get; set; }

        public int? AppoinmentScheduleDay { get; set; }

        public string AppoinmentMessageDeliveryDateTime { get; set; }


    }

    public class EnrollmentDetail
    {
        public int User_ID { get; set; }
        public string User_Name { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public DateTime? NextAppointment { get; set; }
        public int? TriggerEventId { get; set; }
        public int MessageID { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }

        public string ScheduledName { get; set; }
        public string TriggerEventNum { get; set; }
        public DateTime? Date { get; set; }

        public string EnrollmentMessageDate { get; set; }
        public string Time { get; set; }


    }

    public class UserEnrollments
    {
        public string message { get; set; }
        public DateTime date { get; set; }
    }
}