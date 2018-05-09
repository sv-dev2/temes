using DevinTwilio.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DevinTwilio.Models
{
    public class ScheduleViewModels
    {

        public ScheduleViewModels()
        {
            MMSubDomainId = 0;
            MMDomainId = 0;
        }

        public int ScheduleMessageID { get; set; }
        public int? DisplayMessageID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Time { get; set; }
       [RegularExpression("^-?[0-9]?$", ErrorMessage ="Day accepts only numbers")]
        public int? Day { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        //[ForeignKey("TriggerType")]
        public int TriggerTypeId { get; set; }

        public string TriggerTypeName { get; set; }
        public virtual TriggerType TriggerType { get; set;}

        //[Required]
        //[ForeignKey("TriggerEvent")]
        public int TriggerEventId { get; set; }

        public string TriggerEventName { get; set; }
        public string TriggerEventDay { get; set; }
        public virtual TriggerEvent TriggerEvent { get; set; }

        [Required]
        [ForeignKey("Trigger")]
        public int TriggerId { get; set; }
        public virtual Trigger Trigger { get; set; }
        public string TriggerName { get; set; }
        public int ScheduleMessage_MessageId { get; set; }
        public string FrequencyOfDelivery { get; set; }
        public string If1 { get; set; }
        public string If2 { get; set; }
        public string OtherNotes { get; set; }

        public string TimeSent { get; set; }

        public virtual SmsReply Smsreply { get; set; }
        public string To { get; set; }
        public DateTime? SentDate { get; set; }
       // [Required]
        //[ForeignKey("MMDomain")]
        public int MMDomainId { get; set; }
       // public virtual MMDomain MMDomain { get; set; }

        public string MMDomainName { get; set; }

        //[Required]
        //[ForeignKey("MMSubDomain")]
        public int MMSubDomainId { get; set; }
       // public virtual MMSubDomain MMSubDomain { get; set; }
        public string MMSubDomainName { get; set; }

        // Get the user List
        public List<UserViewModels> UsesList { get; set; }

        public string UserName { get; set; }

    }

    public class TriggerType
    {
       public int TriggerTypeId { get; set; }
        public string TriggerTypeName { get; set; }
    }

    public class TriggerEvent
    {
        public int TriggerEventId { get; set; }
        public string TriggerEventName { get; set; }
    }

    public class Trigger
    {
        public int TriggerId { get; set; }
        public string TriggerName { get; set; }
    }

    public class MMDomain
    {
        public int MMDomainId { get; set; }
        public string MMDomainName { get; set; }
    }

    public class MMSubDomain
    {
        public int MMSubDomainId { get; set; }
        public string MMSubDomainName { get; set; }
    }
   
}