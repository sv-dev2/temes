using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevinTwilio.Models
{
    public class SMSReplyViewModels
    {
        public int Id { get; set; }
        public string Sid { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int ParentSid { get; set; }
        public int MessageId { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string MessageResponse { get; set; }
        public string SmsStatus { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string TypeName { get; set; }

    }
}