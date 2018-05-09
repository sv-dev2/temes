using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevinTwilio.Models
{
    public class SMSViewModels
    {
        public int UserID { get; set; }

        public int MessageID { get; set; }
        public string UserName { get; set; }
        public string UserMobileNumber { get; set; }
        public string Message { get; set; }

        public string From { get; set; }
        public string To { get; set; }
        public string Sid { get; set; }
        public string ParentSid { get; set; }
        public string If1 { get; set; }
        public string If2 { get; set; }
        public string EnrollmentDate { get; set; }
    }
}