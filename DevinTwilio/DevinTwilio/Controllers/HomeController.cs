using DevinTwilio.Common;
using DevinTwilio.DAL;
using DevinTwilio.DbLayer;
using DevinTwilio.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using Twilio.Types;


namespace DevinTwilio.Controllers
{
    public class HomeController : Controller
    {
        string ToNumber = string.Empty;
        string digits = String.Empty;
        UserService userService = new UserService();

        [HttpGet]
        public ActionResult Index()
        {

            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<ScheduleViewModels> listScheduleMessages = GetAll();
            return View(listScheduleMessages);
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                //Create a DataTable.
                DataTable dt = new DataTable();
                dt = ConvertCSVtoDataTable(filePath, true);
                DevinTwilioEntities db = new DevinTwilioEntities();

                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int? TriggerTypeID = null;
                        int? TriggerEventID = null;
                        int? TriggerID = null;
                        int? MMDomainID = null;
                        int? MMSubDomainID = null;

                        string TriggerType = Convert.ToString(dt.Rows[i]["Trigger Type"]);
                        if (!string.IsNullOrEmpty(TriggerType))
                        {
                            var TriggerTypeDetail = db.TriggerTypes.Where(m => m.TriggerType_Type == TriggerType).FirstOrDefault();

                            if (TriggerTypeDetail != null)
                            {
                                TriggerTypeID = TriggerTypeDetail.TriggerType_ID;
                            }

                        }


                        string TriggerEventDay = Convert.ToString(dt.Rows[i]["Trigger Event"]);
                        if (!string.IsNullOrEmpty(TriggerEventDay) && TriggerTypeID != null && TriggerTypeID > 0)
                        {
                            var TriggerEventDetails = db.TriggerEvents.Where(c => c.TriggerEvent_Day == TriggerEventDay && c.TriggerTypeId == TriggerTypeID).FirstOrDefault();
                            if (TriggerEventDetails != null)
                            {
                                TriggerEventID = TriggerEventDetails.TriggerEvent_ID;
                            }
                        }


                        //string Trigger = Convert.ToString(dt.Rows[i]["Trigger"]);
                        //if (!string.IsNullOrEmpty(Trigger))
                        //{
                        //    var TriggersDetails = db.Triggers.Where(c => c.Trigger_Trigger == Trigger).FirstOrDefault();
                        //    if (TriggersDetails != null)
                        //    {
                        //        TriggerID = TriggersDetails.Trigger_ID;
                        //    }
                        //}


                        string MMDomain = Convert.ToString(dt.Rows[i]["MM Domain"]);
                        if (!string.IsNullOrEmpty(MMDomain))
                            MMDomainID = db.MMDomains.Where(c => c.MMDomain_Name == MMDomain).FirstOrDefault().MMDomain_ID;

                        string MMSubDomain = Convert.ToString(dt.Rows[i]["MM Sub-Domain"]);
                        if (!string.IsNullOrEmpty(MMSubDomain))
                            MMSubDomainID = db.MMSubDomains.Where(c => c.MMSubDomain_Name == MMSubDomain).FirstOrDefault().MMSubDomain_ID;

                        ScheduleMessage schMessage = new ScheduleMessage();
                        schMessage.ScheduleMessage_Message = Convert.ToString(dt.Rows[i]["Messages"]);

                        string appointmentScheduleDay = Convert.ToString(dt.Rows[i]["Appointment Schedule Day"]);
                        int appoinmentDay = 0;
                        if (appointmentScheduleDay != "")
                        {
                            double num;
                            if (double.TryParse(appointmentScheduleDay, out num))
                            {
                                appoinmentDay = Convert.ToInt32(appointmentScheduleDay);
                                schMessage.Appointment_Schedule = appoinmentDay;
                            }
                        }

                        if (dt.Rows[i]["ID"] != null)
                            schMessage.Display_MessageId = Convert.ToInt32(dt.Rows[i]["ID"]);


                        schMessage.ScheduleMessage_TriggerTypeID = TriggerTypeID;
                        schMessage.ScheduleMessage_TriggerEventID = TriggerEventID;
                        // schMessage.ScheduleMessage_TriggerID = TriggerID;
                        schMessage.ScheduleMessage_Time = Convert.ToString(dt.Rows[i]["Time"]);
                        //   schMessage.ScheduleMessage_FrequencyOfDelivery = Convert.ToString(dt.Rows[i]["Frequency of delivery"]);
                        schMessage.ScheduleMessage_If1 = Convert.ToString(dt.Rows[i]["If 1"]);
                        schMessage.ScheduleMessage_If2 = Convert.ToString(dt.Rows[i]["If 2"]);
                        //  schMessage.ScheduleMessage_OtherNotes = Convert.ToString(dt.Rows[i]["Other Notes"]);
                        schMessage.ScheduleMessage_TimesSent = Convert.ToString(dt.Rows[i]["Times Sent"]);
                        schMessage.ScheduleMessage_MMDomainID = MMDomainID;
                        schMessage.ScheduleMessage_MMSubDomainID = MMSubDomainID;

                        if (!string.IsNullOrEmpty(schMessage.ScheduleMessage_Message))
                        {
                            db.ScheduleMessages.Add(schMessage);
                            db.SaveChanges();

                            //var dbMessage = new ScheduleMessage();
                            //if (TriggerTypeID == (int)TriggerType_Enum.Enrollment)
                            //    dbMessage = db.ScheduleMessages.FirstOrDefault(c => c.ScheduleMessage_TriggerEventID == TriggerEventID && c.ScheduleMessage_TriggerTypeID == TriggerTypeID);
                            //else if (TriggerTypeID == (int)TriggerType_Enum.Appointment)
                            //{
                            //    dbMessage = db.ScheduleMessages.FirstOrDefault(c => c.ScheduleMessage_TriggerEventID == TriggerEventID && c.ScheduleMessage_TriggerTypeID == TriggerTypeID && c.Appointment_Schedule == appoinmentDay);
                            //}
                            //else
                            //    dbMessage = db.ScheduleMessages.FirstOrDefault(c => c.ScheduleMessage_Message == schMessage.ScheduleMessage_Message);


                            //if (dbMessage == null || dbMessage.ScheduleMessage_ID == 0)
                            //{
                            //    db.ScheduleMessages.Add(schMessage);
                            //    db.SaveChanges();
                            //}
                        }
                    }
                    TempData["successMessage"] = "Message has been added sucessfully.";
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                }
            }

            List<ScheduleViewModels> listScheduleMessages = GetAll();
            return View(listScheduleMessages);
        }

        public ActionResult MessageDetails(int id = 0)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(GetDetailsByMesssagId(id));
        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath, bool isFirstRowHeader)
        {
            // You can also read from a file
            TextFieldParser parser = new TextFieldParser(strFilePath);
            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");
            string[] headers;
            headers = parser.ReadFields();
            //string[] headers = sr.ReadLine().Split(',');
            DataTable dt = new DataTable();
            foreach (string header in headers)
            {
                dt.Columns.Add(header.Trim('"').Trim('*'));
            }
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = fields[i].Trim('"').Trim('=').Trim('"').Trim('"').Trim('*');
                }
                dt.Rows.Add(dr);
            }

            parser.Close();
            return dt;
        }


        [HttpGet]
        public ActionResult Create()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            BindDropDowns();
            return View();
        }

        [HttpPost]
        public ActionResult Create(ScheduleViewModels model)
        {
            if (model.TriggerTypeId == 4)
            {
                if (model.Day == null)
                {
                    TempData["errorMessage"] = "Day field is required for Appointment type";
                    BindDropDowns();
                    ViewBag.eventId = model.TriggerTypeId;
                    return View();
                }
            }


            if (ModelState.IsValid)
            {
                // Save the data in the Schedule Message
                try
                {
                    DevinTwilioEntities db = new DevinTwilioEntities();

                    var dbResult = new ScheduleMessage();


                    //** This code is commented out to add multiple messages for same event type, event and for same day **//

                    //if (model.TriggerTypeId == (int)TriggerType_Enum.Response)
                    //    dbResult = db.ScheduleMessages.FirstOrDefault(c => c.ScheduleMessage_Message == model.Message);
                    //else if (model.TriggerTypeId == (int)TriggerType_Enum.Appointment)
                    //{
                    //    dbResult = db.ScheduleMessages.FirstOrDefault(c => c.ScheduleMessage_TriggerEventID == model.TriggerEventId && c.ScheduleMessage_TriggerTypeID == model.TriggerTypeId && c.Appointment_Schedule == model.Day);
                    //    }
                    //else
                    //    dbResult = db.ScheduleMessages.FirstOrDefault(c => c.ScheduleMessage_TriggerEventID == model.TriggerEventId && c.ScheduleMessage_TriggerTypeID == model.TriggerTypeId);
                    //if (dbResult != null)
                    //{
                    //    TempData["errorMessage"] = "Message already exist for selected trigger types and events.";
                    //    BindDropDowns();
                    //    ViewBag.eventId = model.TriggerTypeId;
                    //    return View();
                    //}


                    // TODO 1 - we have to remove this code and assign mmdomainid and mmsubdomainid directly from model
                    var domainId = 0;
                    var subDomainId = 0;
                    if (model.MMDomainName != "")
                    {
                        var mmDomain = db.MMDomains.FirstOrDefault(c => c.MMDomain_Name == model.MMDomainName);
                        if (mmDomain != null)
                            domainId = mmDomain.MMDomain_ID;
                    }

                    if (model.MMSubDomainName != "")
                    {
                        var mmSubDomain = db.MMSubDomains.FirstOrDefault(c => c.MMSubDomain_Name == model.MMSubDomainName);
                        if (mmSubDomain != null)
                            subDomainId = mmSubDomain.MMSubDomain_ID;
                    }


                    ScheduleMessage objScheduleMessage = new ScheduleMessage();
                    objScheduleMessage.ScheduleMessage_Message = model.Message;
                    objScheduleMessage.ScheduleMessage_TriggerTypeID = model.TriggerTypeId;
                    objScheduleMessage.ScheduleMessage_TriggerEventID = model.TriggerEventId;
                    objScheduleMessage.ScheduleMessage_TriggerID = model.TriggerId;
                    objScheduleMessage.ScheduleMessage_Time = model.Time;
                    objScheduleMessage.ScheduleMessage_If1 = model.If1;
                    objScheduleMessage.ScheduleMessage_If2 = model.If2;
                    objScheduleMessage.ScheduleMessage_OtherNotes = model.OtherNotes;
                    objScheduleMessage.ScheduleMessage_MMDomainID = domainId;
                    objScheduleMessage.ScheduleMessage_MMSubDomainID = subDomainId;
                    objScheduleMessage.Appointment_Schedule = model.Day;
                    db.ScheduleMessages.Add(objScheduleMessage);
                    db.SaveChanges();
                    TempData["successMessage"] = "Message has been added sucessfully.";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                }
            }
            BindDropDowns();
            return View();
        }

        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            BindDropDowns();

            ScheduleViewModels viewModel = new ScheduleViewModels();

            try
            {
                if (Session["loggedUser"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (id != 0)
                {
                    viewModel = userService.GetMessageById(id);
                }
            }
            catch (Exception ex)
            {

            }


            return View(viewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddOrEdit(ScheduleViewModels model)
        {
            ScheduleViewModels viewModel = new ScheduleViewModels();
            try
            {
                if (model.ScheduleMessageID != 0)
                {
                    var dbResult = userService.GetMessageDetailsById(model.ScheduleMessageID);
                    dbResult.ScheduleMessage_Message = model.Message;
                    dbResult.ScheduleMessage_TriggerTypeID = model.TriggerTypeId;
                    dbResult.ScheduleMessage_TriggerEventID = model.TriggerEventId;
                    dbResult.ScheduleMessage_TriggerID = model.TriggerId;
                    dbResult.ScheduleMessage_If1 = model.If1;
                    dbResult.ScheduleMessage_If2 = model.If2;
                    dbResult.ScheduleMessage_Time = model.Time;
                    dbResult.Appointment_Schedule = model.Day;
                    if (userService.UpdateMessage(dbResult))
                    {
                        TempData["successMessage"] = "Message has been updated sucessfully.";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            BindDropDowns();
            return View(viewModel);
        }

        [AcceptVerbs("Get", "Post")]
        public ActionResult SMSReply()
        {
            const string accountSid = "ACb61b5dc2ad53c6336667449e67302c48";
            const string authToken = "b1dfd0ebd09b0bd2d840f07618b33a27";
            TwilioClient.Init(accountSid, authToken);
            string messageToBeSentIf1 = "";
            string messageToBeSentIf2 = "";

            var filename = Server.MapPath("~/LogTextFile/logErrors.txt");
            var sw = new System.IO.StreamWriter(filename, true);



            try
            {
                // get the body of the message
                // get the no where we are getting the reply, then forward the message againwith that no
                using (DevinTwilioEntities dc = new DevinTwilioEntities())
                {
                    sw.WriteLine("Inside SMS Reply function at " + DateTime.Now.ToString());
                    var _toNumber = Request["From"];

                    sw.WriteLine("From = " + _toNumber);
                    var _body = Request["Body"];

                    sw.WriteLine("_body = " + _body);
                    var _fromNumber = Request["To"];

                    sw.WriteLine("To = " + _fromNumber);
                    var MessagesSid = Request["MessageSid"];

                    //  var message = MessageResource.Fetch(MessagesSid);
                    //  message.sm

                    var SmsSid = Request["SmsSid"];
                    var MessagingServiceSid = Request["MessagingServiceSid"];

                    sw.WriteLine("MessagesSid = " + MessagesSid);
                    sw.WriteLine("SmsSid = " + SmsSid);
                    sw.WriteLine("MessagingServiceSid = " + MessagingServiceSid);
                    //sw.WriteLine("Sid = " + message.Sid);
                    sw.WriteLine("Outside main code at _toNumber = " + _toNumber + "_body = " + _body + "_fromNumber = " + _fromNumber + "MessagesSid = " + MessagesSid);

                    //var _toNumber = Request["From"];
                    //var _body = Request["Body"];
                    //var _fromNumber = Request["To"];
                    //var MessagesSid = Request["MessageSid"];

                    //  Request["SmsStatus"]    //    _toNumber = "+917986501164";
                    //  _fromNumber = "+16467591379";
                    //  string toNumber = Request["From"]; // new PhoneNumber("+917986501164");
                    //  string fromNumber = Request["To"]; // new PhoneNumber("+16467591379");

                    // Get the record specific to phone number (From and To) and messageSId SMS response and status

                    //var dbResult = new SmsReply();

                    //if (!string.IsNullOrEmpty(SmsSid))
                    //    dbResult = dc.SmsReplies.OrderByDescending(x => x.Id).Where(x => x.Sid == SmsSid && x.From == _fromNumber && x.To == _toNumber).FirstOrDefault();

                    //if(dbResult!=null && dbResult.Id==0)
                     var   dbResult = dc.SmsReplies.OrderByDescending(x => x.Id).Where(x => x.From == _fromNumber && x.To == _toNumber).FirstOrDefault();


                    if (dbResult != null)
                    {
                        int? msgId = dbResult.MessageId;
                        sw.WriteLine("Inside dbResult at " + DateTime.Now.ToString());
                        var scheduledMessage = dc.ScheduleMessages.FirstOrDefault(c => c.ScheduleMessage_ID == msgId);

                        dbResult.SmsStatus = Request["SmsStatus"];
                        if ((_body == "1" || _body == "\"1\""))
                        {
                            dbResult.MessageResponse = scheduledMessage.ScheduleMessage_If1;
                            messageToBeSentIf1 = scheduledMessage.ScheduleMessage_If1;
                        }
                        else if ((_body == "2" || _body == "\"2\""))
                        {
                            dbResult.MessageResponse = scheduledMessage.ScheduleMessage_If2;
                            messageToBeSentIf2 = scheduledMessage.ScheduleMessage_If2;
                        }

                        dbResult.ResponseDate = DateTime.Now;
                        dbResult.MessageResponse = _body;
                        sw.WriteLine("dbResult MessageResponse " + dbResult.MessageResponse);
                        dc.SaveChanges();

                    }

                    //var data = (from i in dc.SmsReplies
                    //            join k in dc.ScheduleMessages on i.MessageId equals k.ScheduleMessage_ID
                    //            where i.From == _fromNumber && i.To == _toNumber
                    //            orderby i.Id descending
                    //            select new SMSViewModels
                    //            {
                    //                MessageID = (int)i.MessageId,
                    //                Message = k.ScheduleMessage_Message,
                    //                If1 = k.ScheduleMessage_If1,
                    //                If2 = k.ScheduleMessage_If2
                    //            }).ToList().FirstOrDefault();

                    //if (data != null)
                    //{
                    //    if (!string.IsNullOrEmpty(data.If1))
                    //    {
                    //        messageToBeSentIf1 = data.If1;
                    //    }

                    //    if (!string.IsNullOrEmpty(data.If2))
                    //    {
                    //        messageToBeSentIf2 = data.If2;
                    //    }
                    //}

                    sw.WriteLine("messageToBeSentIf1 " + messageToBeSentIf1);
                    sw.WriteLine("messageToBeSentIf2 " + messageToBeSentIf2);

                    if (string.IsNullOrEmpty(messageToBeSentIf1))
                    {
                        messageToBeSentIf1 = "Invalid request";
                    }
                    if (string.IsNullOrEmpty(messageToBeSentIf2))
                    {
                        messageToBeSentIf2 = "Invalid request";
                    }

                    if (!string.IsNullOrEmpty(_body))
                    {
                        if ((_body == "1" || _body == "\"1\"") && !string.IsNullOrEmpty(messageToBeSentIf1))
                        {
                            sw.WriteLine("Inside Request Body 1 Block response (Before Creating Message) " + " at " + DateTime.Now.ToString());
                            //sw.WriteLine("to = " + _fromNumber + "From = " + _toNumber + "body = " + messageToBeSentIf1 + "at " + DateTime.Now.ToString());
                            sw.WriteLine("to = " + _toNumber + "From = " + _fromNumber + "body = " + messageToBeSentIf1 + "at " + DateTime.Now.ToString());
                            MessageResource.Create(to: _toNumber, from: _fromNumber, body: messageToBeSentIf1);
                            //   MessageResource.Create(to: _fromNumber, from: _toNumber, body: messageToBeSentIf1);
                            sw.WriteLine("Inside Request Body 1 Block response (After Creating Message) " + " at " + DateTime.Now.ToString());
                        }
                        else if ((_body == "2" || _body == "\"2\"") && !string.IsNullOrEmpty(messageToBeSentIf2))
                        {
                            sw.WriteLine("Inside Request Body 2 Block response (Before Creating Message) " + " at " + DateTime.Now.ToString());
                            //   sw.WriteLine("to = "+ _fromNumber +"From = "+ _toNumber + "body = "+ messageToBeSentIf2 + "at " + DateTime.Now.ToString());
                            sw.WriteLine("to = " + _toNumber + "From = " + _fromNumber + "body = " + messageToBeSentIf2 + "at " + DateTime.Now.ToString());
                            MessageResource.Create(to: _toNumber, from: _fromNumber, body: messageToBeSentIf2);
                            //     MessageResource.Create(to: _fromNumber, from: _toNumber, body: messageToBeSentIf2);
                            sw.WriteLine("Inside Request Body 2 Block response (After Creating Message) " + " at " + DateTime.Now.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sw.WriteLine("Exception" + ex.Message + "at " + DateTime.Now.ToString());
                WriteXmltoResponseString("Exception: " + ex.Message, new string[1] { GetRedirectUrl(Request.Url.ToString()) });
            }
            finally
            {
                sw.Dispose();
                sw.Close();
            }
            return View();
        }
        public ActionResult MessageTree()
        {
            //if (Session["loggedUser"] == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            using (DevinTwilioEntities dc = new DevinTwilioEntities())
            {
                var data = (from u in dc.SmsReplies
                            select new SMSReplyViewModels
                            {
                                MessageId = (int)u.MessageId,
                                Message = dc.ScheduleMessages.Where(x => x.ScheduleMessage_ID == u.MessageId).Select(x => x.ScheduleMessage_Message).FirstOrDefault(),
                                From = u.From,
                                To = u.To,
                                UserName = dc.Users.Where(x => x.User_ID == u.UserId).Select(x => x.User_Name).FirstOrDefault(),
                                MessageResponse = u.MessageResponse,
                                SmsStatus = u.SmsStatus
                            }).ToList();
                return View(data);
            }
        }

        [HttpPost]
        public ActionResult BindTriggerEventDropDown(int id)
        {
            DevinTwilioEntities db = new DevinTwilioEntities();
            IEnumerable<SelectListItem> triggerEventDataList = db.TriggerEvents.Where(c => c.TriggerTypeId == id && c.TriggerEvent_Day != null).OrderByDescending(c => c.TriggerEvent_ID).AsEnumerable().Select(x =>
                                    new SelectListItem
                                    {
                                        Value = x.TriggerEvent_ID.ToString(),
                                        Text = x.TriggerEvent_Day
                                    }).ToList();
            return Json(triggerEventDataList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MessageTree(UserViewModels model)
        {
            const string accountSid = "ACb61b5dc2ad53c6336667449e67302c48";
            const string authToken = "b1dfd0ebd09b0bd2d840f07618b33a27";
            TwilioClient.Init(accountSid, authToken);
            using (DevinTwilioEntities dc = new DevinTwilioEntities())
            {
                var users = dc.Users.Where(c => c.EnrollmentDate != null).ToList();
                var list = (from schMessage in dc.ScheduleMessages
                            join trgEvent in dc.TriggerEvents
                            on schMessage.ScheduleMessage_TriggerEventID equals trgEvent.TriggerEvent_ID
                            select new ScheduleViewModels { Message = schMessage.ScheduleMessage_Message, TriggerEventName = trgEvent.TriggerEvent_Event, ScheduleMessageID = schMessage.ScheduleMessage_ID }).ToList();

                try
                {
                    foreach (var item in users)
                    {
                        var to = new PhoneNumber(item.MobileNumber.ToString());
                        var from = new PhoneNumber("+16467591379");
                        var EnrollmentDate = item.EnrollmentDate;
                        if (EnrollmentDate != null)
                        {
                            DateTime? currentDate = DateTime.Now;
                            TimeSpan t = (TimeSpan)(currentDate - EnrollmentDate);
                            int totalDays = t.Days;
                            if (totalDays >= 0 && totalDays <= 50)
                            {
                                string Sid = String.Empty;
                                var messageDetails = list.FirstOrDefault(c => c.TriggerEventName.ToLower() == "day " + totalDays);
                                if (messageDetails != null)
                                {
                                    var message0 = MessageResource.Create(to: to, from: from, body: messageDetails.Message);
                                    Sid = Convert.ToString(message0.Sid);
                                    if (!string.IsNullOrEmpty(Sid))
                                    {
                                        SmsReply _smsReply = new SmsReply();
                                        _smsReply.From = Convert.ToString(from);
                                        _smsReply.To = Convert.ToString(to);
                                        _smsReply.Sid = Convert.ToString(Sid);
                                        _smsReply.ParentSid = null;
                                        _smsReply.MessageId = messageDetails.ScheduleMessageID;
                                        _smsReply.UserId = item.User_ID;
                                        dc.SmsReplies.Add(_smsReply);
                                        dc.SaveChanges();
                                    }
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }


            }
            //  var call = CallResource.Create(to,from,url: new Uri("http://devintwilio.kindlebit.com/home/SMSReply"));
            using (DevinTwilioEntities dc = new DevinTwilioEntities())
            {
                var data = (from u in dc.SmsReplies
                            select new SMSReplyViewModels
                            {
                                MessageId = (int)u.MessageId,
                                Message = dc.ScheduleMessages.Where(x => x.ScheduleMessage_ID == u.MessageId).Select(x => x.ScheduleMessage_Message).FirstOrDefault(),
                                From = u.From,
                                To = u.To,
                                UserName = dc.Users.Where(x => x.User_ID == u.UserId).Select(x => x.User_Name).FirstOrDefault(),
                                MessageResponse = u.MessageResponse,
                                SmsStatus = u.SmsStatus
                            }).ToList();
                return View(data);
            }
        }

        private void WriteXmltoResponseString(string xml, string[] parameters)
        {
            try
            {
                string xmlData = string.Format(xml, parameters);
                Response.Write(xmlData);
                Response.Flush();
                Response.SuppressContent = true;
            }
            catch (Exception ex)
            { }
        }

        public string GetRedirectUrl(String Url)
        {
            if (Url.Length > 0)
            {
                String[] urlArr = Url.Split('/');
                Url = urlArr[0] + "//";
                if (urlArr[2].Contains(":"))
                {
                    Url = Url + urlArr[2];
                }
                else
                {
                    Url = Url + urlArr[2] + "/" + urlArr[3];
                }
            }
            return Url;
        }

        public void BindDropDowns()
        {
            DevinTwilioEntities db = new DevinTwilioEntities();
            // Get User Data List
            //IEnumerable<SelectListItem> userRolesrDataList = db.Roles.Where(c=>c.Role_Name!="Admin").AsEnumerable().Select(x =>
            //            new SelectListItem
            //            {
            //                Value = x.Role_Id.ToString(),
            //                Text = x.Role_Name
            //            });
            //ViewBag.AssignedUserRoles = userRolesrDataList;
            // Get Trigger Type List
            IEnumerable<SelectListItem> triggerTypeDataList = db.TriggerTypes.AsEnumerable().Select(x =>
                        new SelectListItem
                        {
                            Value = x.TriggerType_ID.ToString(),
                            Text = x.TriggerType_Type
                        });
            ViewBag.TriggerTypes = triggerTypeDataList;
            // Get Trigger Event List
            //IEnumerable<SelectListItem> triggerEventDataList = db.TriggerEvents.AsEnumerable().Select(x =>
            //            new SelectListItem
            //            {
            //                Value = x.TriggerEvent_ID.ToString(),
            //                Text = x.TriggerEvent_Event
            //            });
            //ViewBag.TriggerEvents = triggerEventDataList;
            // Get Trigger List
            IEnumerable<SelectListItem> triggerDataList = db.Triggers.AsEnumerable().Select(x =>
                        new SelectListItem
                        {
                            Value = x.Trigger_ID.ToString(),
                            Text = x.Trigger_Trigger
                        });
            ViewBag.Triggers = triggerDataList;
            // Get MM Domain List
            IEnumerable<SelectListItem> mmDomainDataList = db.MMDomains.AsEnumerable().Select(x =>
                        new SelectListItem
                        {
                            Value = x.MMDomain_ID.ToString(),
                            Text = x.MMDomain_Name
                        });
            ViewBag.MMDomains = mmDomainDataList;
            // Get MM Sub Domain List
            IEnumerable<SelectListItem> mmSubDomainDataList = db.MMSubDomains.AsEnumerable().Select(x =>
                        new SelectListItem
                        {
                            Value = x.MMSubDomain_ID.ToString(),
                            Text = x.MMSubDomain_Name
                        });
            ViewBag.MMSubDomains = mmSubDomainDataList;
            // Get Stage InTX List
            IEnumerable<SelectListItem> stageInTXDataList = db.StageInTXes.AsEnumerable().Select(x =>
                        new SelectListItem
                        {
                            Value = x.StageInTX_ID.ToString(),
                            Text = x.StageInTX_Name
                        });
            ViewBag.Stages = stageInTXDataList;
        }

        public List<ScheduleViewModels> GetAll()
        {
            DevinTwilioEntities dbConn = new DevinTwilioEntities();
            var results = from i in dbConn.ScheduleMessages
                          join k in dbConn.TriggerTypes on i.ScheduleMessage_TriggerTypeID equals k.TriggerType_ID into trgTypes
                          from tType in trgTypes.DefaultIfEmpty()
                          join l in dbConn.TriggerEvents on i.ScheduleMessage_TriggerEventID equals l.TriggerEvent_ID into trgEvent
                          from tEvent in trgEvent.DefaultIfEmpty()
                              //join m in dbConn.Triggers on i.ScheduleMessage_TriggerTypeID equals m.Trigger_ID into trg
                              //from tr in trg.DefaultIfEmpty()
                          join n in dbConn.MMDomains on i.ScheduleMessage_MMDomainID equals n.MMDomain_ID into mmDomain
                          from md in mmDomain.DefaultIfEmpty()
                          join o in dbConn.MMSubDomains on i.ScheduleMessage_MMSubDomainID equals o.MMSubDomain_ID into mmSubDoamin
                          from mSd in mmSubDoamin.DefaultIfEmpty()
                          join sms in dbConn.SmsReplies on i.ScheduleMessage_ID equals sms.MessageId into smsReplies
                          from sMs in smsReplies.DefaultIfEmpty()
                          join user in dbConn.Users on sMs.UserId equals user.User_ID into userL
                          from user in userL.DefaultIfEmpty()
                              // orderby i.ScheduleMessage_ID descending
                          select new ScheduleViewModels
                          {
                              ScheduleMessageID = i.ScheduleMessage_ID,
                              DisplayMessageID = i.Display_MessageId == null ? 0 : i.Display_MessageId,
                              Message = i.ScheduleMessage_Message,
                              FrequencyOfDelivery = i.ScheduleMessage_FrequencyOfDelivery,
                              If1 = i.ScheduleMessage_If1,
                              If2 = i.ScheduleMessage_If2,
                              OtherNotes = i.ScheduleMessage_OtherNotes,
                              TimeSent = i.ScheduleMessage_TimesSent,
                              // TriggerTypeName = k.TriggerType_Type,
                              TriggerTypeName = tType.TriggerType_Type,
                              //TriggerEventName = l.TriggerEvent_Event,
                              // TriggerEventName = tEvent.TriggerEvent_Event, // by ash 10 april
                              TriggerEventDay = tEvent.TriggerEvent_Day,
                              // TriggerName = m.Trigger_Trigger,
                              // TriggerName = tr.Trigger_Trigger,
                              MMDomainName = md.MMDomain_Name,
                              MMSubDomainName = mSd.MMSubDomain_Name,
                              To = sMs.To,
                              UserName = user.User_Name,
                              SentDate = sMs.SentDate,
                              Day = i.Appointment_Schedule

                          };
            List<ScheduleViewModels> listScheduleMessages = results.OrderByDescending(c => c.ScheduleMessageID).ToList();
            return listScheduleMessages;
        }

        public ScheduleViewModels GetDetailsByMesssagId(int id)
        {
            DevinTwilioEntities dbConn = new DevinTwilioEntities();
            var results = (from i in dbConn.ScheduleMessages.Where(c => c.ScheduleMessage_ID == id)
                           join k in dbConn.TriggerTypes on i.ScheduleMessage_TriggerTypeID equals k.TriggerType_ID
                           join l in dbConn.TriggerEvents on i.ScheduleMessage_TriggerEventID equals l.TriggerEvent_ID into te
                           from trigEv in te.DefaultIfEmpty()
                               // join m in dbConn.Triggers on i.ScheduleMessage_TriggerTypeID equals m.Trigger_ID
                           join n in dbConn.MMDomains on i.ScheduleMessage_MMDomainID equals n.MMDomain_ID into mm
                           from mmdo in mm.DefaultIfEmpty()
                           join o in dbConn.MMSubDomains on i.ScheduleMessage_MMSubDomainID equals o.MMSubDomain_ID into mms
                           from mmsdo in mms.DefaultIfEmpty()
                               // orderby i.ScheduleMessage_ID descending
                           select new ScheduleViewModels
                           {
                               ScheduleMessageID = i.ScheduleMessage_ID,
                               DisplayMessageID = i.Display_MessageId == null ? 0 : i.Display_MessageId,
                               Message = i.ScheduleMessage_Message,
                               FrequencyOfDelivery = i.ScheduleMessage_FrequencyOfDelivery,
                               If1 = i.ScheduleMessage_If1,
                               If2 = i.ScheduleMessage_If2,
                               OtherNotes = i.ScheduleMessage_OtherNotes,
                               TimeSent = i.ScheduleMessage_TimesSent,
                               TriggerTypeName = k.TriggerType_Type,
                               TriggerEventName = trigEv.TriggerEvent_Event,
                               //TriggerName = m.Trigger_Trigger,
                               MMDomainName = mmdo.MMDomain_Name,
                               MMSubDomainName = mmsdo.MMSubDomain_Name
                           }).FirstOrDefault();
            return results;
        }

        public ActionResult Appointment()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            UserService userService = new UserService();
            AppointmentViewModels viewModel = new AppointmentViewModels();
            viewModel.UsesList = userService.GetAllUsers();
            viewModel.ProviderList = userService.GetAllProvider();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Appointment(AppointmentViewModels model)
        {

            AppointmentViewModels viewModel = new AppointmentViewModels();
            viewModel.UsesList = userService.GetAllUsers();
            viewModel.ProviderList = userService.GetAllProvider();
            Appointment app = new Appointment { Appointment_PatientID = model.UserId, Appointment_ProviderID = model.ProviderId, Appointment_StartTime = model.StartTime, Appointment_EndTime = model.EndTime };
            if (userService.AddAppointment(app))
                return RedirectToAction("Appointment", "Home");

            return View(viewModel);
        }


        public void SendMessageInFrequency()
        {
            const string accountSid = "ACb61b5dc2ad53c6336667449e67302c48";
            const string authToken = "b1dfd0ebd09b0bd2d840f07618b33a27";
            TwilioClient.Init(accountSid, authToken);

            try
            {
                using (DevinTwilioEntities _context = new DevinTwilioEntities())
                {
                    var list = (from appoint in _context.Appointments
                                join user in _context.Users on appoint.Appointment_PatientID equals user.User_ID
                                join msg in _context.ScheduleMessages on appoint.Appointment_Trigger_EventId equals msg.ScheduleMessage_TriggerEventID
                                join trgEventTypes in _context.TriggerEvents on appoint.Appointment_Trigger_EventId equals trgEventTypes.TriggerEvent_ID
                                // join trigg in _context.Triggers on msg.ScheduleMessage_TriggerID equals trigg.Trigger_ID
                                // join triggtype in _context.TriggerTypes on msg.ScheduleMessage_TriggerTypeID equals triggtype.TriggerType_ID
                                select new MessageFrequencyModel
                                {
                                    Message = msg.ScheduleMessage_Message,
                                    UserName = user.User_Name,
                                    MobileNum = user.MobileNumber,
                                    // Trigger = trigg.Trigger_Trigger,
                                    TriggerId = (int)msg.ScheduleMessage_TriggerID,
                                    // TriggerType = triggtype.TriggerType_Type,
                                    AppointmentDate = (DateTime)appoint.Appointment_StartTime,
                                    MobileNumber = user.MobileNumber,
                                    AppoinmentType = trgEventTypes.TriggerEvent_Event,
                                    Time = msg.ScheduleMessage_Time,
                                    AppointmentId = appoint.Appointment_ID
                                }).Distinct().ToList();

                    var distnctMessageList = new List<MessageFrequencyModel>();
                    foreach (var item in list)
                    {
                        if (distnctMessageList.FirstOrDefault(c => c.AppointmentId == item.AppointmentId) == null)
                        {
                            distnctMessageList.Add(item);
                        }
                    }

                    foreach (var item in distnctMessageList)
                    {
                        var to = new PhoneNumber(item.MobileNumber.ToString());
                        var from = new PhoneNumber("+16467591379");

                        if ((item.AppointmentDate != null) && (!string.IsNullOrEmpty(item.AppoinmentType)))
                        {
                            DateTime currentAppointmentDate = Convert.ToDateTime(item.AppointmentDate);
                            DateTime currentDate = DateTime.Now;
                            int dayDiff = Convert.ToInt32((currentAppointmentDate - currentDate).TotalDays);
                            //    var calculatedTime = currentDate.ToString("hh:mm:ss tt"); // 7:00 AM // 12 hour clock
                            var calculatedTime = currentDate.ToString("htt");
                            var msgToBeSend = item.Message;
                            var setTime = item.Time == null ? "9am" : item.Time;


                            if (calculatedTime.ToLower() == setTime.ToLower())
                            {
                                // Appointcondition wll be for 1 day and 7  day before
                                if (dayDiff == 1 || dayDiff == 7)
                                {
                                    MessageResource.Create(to: to, from: from, body: msgToBeSend);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult DashboardText()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            DashoboardModel model = userService.GetDashboardDetail();

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DashboardText(DashoboardModel model)
        {
            if (model.Id == 0)
            {
                userService.SaveDashboardDetail(model);
            }
            else
            {
                userService.UpdateDashboardDetail(model);
            }

            return RedirectToAction("DashboardText");
        }

        public ActionResult _EventTypeView()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            UserService _context = new UserService();


            return View(_context.GetAllEvents());
        }
        public ActionResult CreateEventType()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            EventViewModels model = new EventViewModels();
            model.TriggerTypeList = userService.GetAllTriggerTypes();
            return View(model);
        }
        [HttpPost]
        public ActionResult CreateEventType(EventViewModels model)
        {
            if (ModelState.IsValid)
            {
                //if (!userService.CheckAppointmentExistForPatientOrNot(model.UserId))
                //{
                //    TempData["ErrorMessage"] = "Event already exist.";
                //    return View(model);
                //}

                DAL.TriggerEvent eventType = new DAL.TriggerEvent
                {
                    TriggerEvent_Event = model.EventName,
                    TriggerTypeId = model.TriggerTypeId

                };
                if (userService.AddEventType(eventType))
                    return RedirectToAction("_EventTypeView");

            }
            model.TriggerTypeList = userService.GetAllTriggerTypes();
            return View(model);
        }

        public ActionResult AddOrEditTriggerEvent(int id = 0)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            EventViewModels viewModel = new EventViewModels();
            if (id != 0)
            {
                viewModel = userService.GetTriggerEventById(id);
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddOrEditTriggerEvent(EventViewModels model)
        {
            EventViewModels viewModel = new EventViewModels();

            //if (ModelState.IsValid)
            //{

            if (model.Id != 0)
            {
                var dbResult = userService.GetTriggerEventDetailsById(model.Id);
                dbResult.TriggerEvent_Event = model.EventName;
                if (userService.UpdateTriggerEvent(dbResult))
                    return RedirectToAction("_EventTypeView");
            }

            //}

            return View(viewModel);
        }

        public ActionResult DeleteTriggerEvent(int id = 0)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            userService.DeleteTriggerEvent(id);
            return RedirectToAction("_EventTypeView");
        }

        public ActionResult DeleteMessage(int id = 0)
        {
            userService.DeleteMessage(id);
            return RedirectToAction("Index");
        }

        public ActionResult _SentMessagesDetails(int id)
        {

            UserService _context = new UserService();
            return View(_context.GetSentMessagesDetailByMessageId(id));
        }

    }
}