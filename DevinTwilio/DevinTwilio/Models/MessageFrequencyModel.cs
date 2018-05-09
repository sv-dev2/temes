using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevinWindowService.Models
{
  public  class MessageFrequencyModel
    {
        public int MessageId { get; set; }
        public string Message { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string MobileNum { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string TriggerType { get; set; }
        public string Trigger { get; set; }
        public int TriggerId { get; set; }
        public string Time { get; set; }
        public string EnrollMentDate { get; set; }
        public string MobileNumber { get; set; }

        public int AppointmentTypeId { get; set; }
        public string AppoinmentType { get; set; }
        public int AppointmentId { get; set; }
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
