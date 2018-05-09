using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevinTwilio.Models
{
    public class MessageFrequencyModel
    {
        public string Message { get; set; }
        public string UserName { get; set; }
        public string MobileNum { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string TriggerType { get; set; }
        public string Trigger { get; set; }
        public int? TriggerId { get; set; }
        public string Time { get; set; }
        public string EnrollMentDate { get; set; }
        public string MobileNumber { get; set; }

        public int AppointmentTypeId { get; set; }
        public string AppoinmentType { get; set; }
        public string TriggerEventDay { get; set; }
        public int AppointmentId { get; set; }

        public int MessageId { get; set; }
        public int UserId { get; set; }

        public int? StatusId { get; set; }

        public int? Day { get; set; }
        public int Appointment_Trigger_EventId { get; set; }

        public int? TriggerEventId { get; set; }
    }

    enum FrequencyDays
    {
        Sevendaysbefore1daybefore = 1,
        Onedaybefore = 2,
        Dayof = 3,
        Immediately = 4,
        Twohoursafter = 5,
        Dayafter = 6,
    }


   


}