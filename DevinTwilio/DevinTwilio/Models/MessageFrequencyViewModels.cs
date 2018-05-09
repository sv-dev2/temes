using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevinTwilio.Models
{
    public class MessageFrequencyViewModels
    {
        public string Message { get; set; }
        public string UserName { get; set; }
        public string MobileNum { get; set; }
        public string AppointmentDate { get; set; }
        public string TriggerType { get; set; }
        public string Trigger { get; set; }
        public int TriggerId { get; set; }
        public string Time { get; set; }
        public string EnrollMentDate { get; set; }
    }
}