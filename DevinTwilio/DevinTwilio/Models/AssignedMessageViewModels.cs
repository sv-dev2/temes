using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevinTwilio.Models
{
    public class AssignedMessageViewModels
    {
        public int MessageID { get; set; }
        public string Message { get; set; }
        public string Frequency { get; set; }

        public int UserID { get; set; }
        public string AssignedUser { get; set; }
    }
}