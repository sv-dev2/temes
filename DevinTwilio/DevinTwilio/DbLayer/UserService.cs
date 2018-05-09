using DevinTwilio.Common;
using DevinTwilio.DAL;
using DevinTwilio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevinTwilio.DbLayer
{

    public class UserService
    {
        DevinTwilioEntities _context = new DevinTwilioEntities();

        public int EnrollmentEventId = 5;
        public int ApointmentEventId = 4;

        public UserViewModels GetUserDetail(string username, string password)
        {

            var userDetails = (from us in _context.Users.Where(c => c.User_Name == username && c.Password == password)
                               join role in _context.Roles.Where(c => c.Role_Name == "Admin")
                               on us.Role_Id equals role.Role_Id
                               select new UserViewModels { User_ID = us.User_ID, EmailAddress = us.EmailAddress, User_Name = us.User_Name, Role_Id = role.Role_Id, RoleName = role.Role_Name }).FirstOrDefault();

            // var userDetails = _context.Users.FirstOrDefault(c => c.User_Name == username && c.Password == password);

            return userDetails;
        }

        public List<UserViewModels> GetAllUsers()
        {
            List<UserViewModels> userList = new List<UserViewModels>();
            using (DevinTwilioEntities db = new DevinTwilioEntities())
            {
                userList = (from user in db.Users.DefaultIfEmpty()
                            join sms in db.SmsReplies on user.User_ID equals sms.UserId
                           //where user.User_Name != "Admin"
                           into t
                            from rt in t.DefaultIfEmpty()
                            orderby rt.SentDate descending

                            select new UserViewModels
                            {
                                User_ID = user.User_ID,
                                User_Name = user.User_Name,
                                EmailAddress = user.EmailAddress,
                                User_PhoneNumber = user.User_PhoneNumber,
                                MobileNumber = user.MobileNumber,
                                CreatedDate = (DateTime)user.CreatedDate
                            }).OrderByDescending(c => c.User_ID).ToList();
            }
            userList = userList.Where(x => x.User_Name != "admin").ToList();
            List<UserViewModels> newList = new List<UserViewModels>();

            foreach (var item in userList)
            {
                var users = userList.Where(c => c.User_ID == item.User_ID).FirstOrDefault();
                var user = newList.Where(c => c.User_ID == item.User_ID).FirstOrDefault();

                if (user == null)
                {
                    newList.Add(users);
                }
            }

            return newList;
        }
        public List<UserViewModels> GetAllPatients()
        {
            List<UserViewModels> userList = new List<UserViewModels>();
            var currentDate = System.DateTime.Now;
            using (DevinTwilioEntities db = new DevinTwilioEntities())
            {
                userList = (from user in db.Users.Where(c => c.User_Name != "Admin" && c.EnrollmentDate != null)
                            join appoint in db.Appointments on user.User_ID equals appoint.Appointment_PatientID
                            into app
                            from ap in app.DefaultIfEmpty()
                                //  join smsreply in _context.SmsReplies on user.User_ID equals smsreply.UserId
                            select new UserViewModels
                            {
                                User_ID = user.User_ID,
                                User_Name = user.User_Name,
                                EmailAddress = user.EmailAddress,
                                User_PhoneNumber = user.User_PhoneNumber,
                                MobileNumber = user.MobileNumber,
                                EnrollmentDate = user.EnrollmentDate,
                                CreatedDate = user.CreatedDate,
                                Notes = user.Notes,
                                LastMessage = db.SmsReplies.Where(x => x.UserId == user.User_ID).OrderByDescending(x => x.Id).Select(x => db.ScheduleMessages.Where(y => y.ScheduleMessage_ID == x.MessageId).Select(y => y.ScheduleMessage_Message).FirstOrDefault()).FirstOrDefault(),
                                // NextAppointment = (DateTime)db.Appointments.Where(x => x.Appointment_StartTime > currentDate).Select(x => x.Appointment_StartTime).FirstOrDefault()
                                NextAppointment = ap.Appointment_StartTime
                            }).ToList();

            }
            return userList.OrderByDescending(c => c.CreatedDate).ToList();
        }

        public List<UserViewModels> GetAllLogs()
        {
            List<UserViewModels> userList = new List<UserViewModels>();
            userList = (from user in _context.Users.Where(c => c.User_Name != "Admin")
                        join smsreply in _context.SmsReplies on user.User_ID equals smsreply.UserId
                        join sch in _context.ScheduleMessages on smsreply.MessageId equals sch.ScheduleMessage_ID
                        select new UserViewModels
                        {
                            User_ID = user.User_ID,
                            User_Name = user.User_Name,
                            Message = sch.ScheduleMessage_Message,
                            Response = smsreply.MessageResponse,
                            Status = smsreply.SmsStatus
                        }).ToList();
            return userList;
        }

        public List<UserViewModels> GetAllLogsByPatient(string patientname)
        {
            List<UserViewModels> userList = new List<UserViewModels>();
            userList = (from user in _context.Users.Where(c => c.User_Name != "Admin")
                        join smsreply in _context.SmsReplies on user.User_ID equals smsreply.UserId
                        join sch in _context.ScheduleMessages on smsreply.MessageId equals sch.ScheduleMessage_ID
                        where user.User_Name == patientname
                        select new UserViewModels
                        {
                            User_ID = user.User_ID,
                            User_Name = user.User_Name,
                            Message = sch.ScheduleMessage_Message,
                            Response = smsreply.MessageResponse,
                            Status = smsreply.SmsStatus,
                            SentDate = smsreply.SentDate,
                            ResponseDate = smsreply.ResponseDate
                        }).ToList();
            return userList;
        }

        public bool CheckAppointmentExistForPatientOrNot(int? patientId)
        {
            var appointment = _context.Appointments.FirstOrDefault(c => c.Appointment_PatientID == patientId);
            if (appointment == null)
                return true;
            else
                return false;
        }
        public bool CheckUserNameExistOrNot(string username)
        {
            var u = _context.Users.FirstOrDefault(c => c.User_Name == username);
            if (u == null)
                return true;
            else
                return false;
        }

        public bool CheckProviderNameExistOrNot(string providername)
        {
            var u = _context.Providers.FirstOrDefault(c => c.ProviderName == providername);
            if (u == null)
                return true;
            else
                return false;
        }

        public UserViewModels GetUserById(int id)
        {
            var result = (from u in _context.Users.Where(c => c.User_ID == id)
                          select new UserViewModels
                          {
                              User_ID = u.User_ID,
                              User_Name = u.User_Name,
                              MobileNumber = u.MobileNumber,
                              EmailAddress = u.EmailAddress,
                              LastLoggedIn = (DateTime)u.LastLoggedIn,
                              EnrollmentDate = u.EnrollmentDate,
                              StatusId = u.StatusId,
                              Notes = u.Notes
                          }).FirstOrDefault();
            return result;
        }

        public User GetUserDetailById(int id = 0)
        {
            return _context.Users.FirstOrDefault(c => c.User_ID == id);

        }

        public void AddUserEnrollment(int userId, int enrollmentId, string action)
        {
            UserEnrollment enrollment = new UserEnrollment();
            enrollment.UserId = userId;
            enrollment.UserEnrollmentId = enrollmentId;
            enrollment.Status = action;

            _context.UserEnrollments.Add(enrollment);
            _context.SaveChanges();


        }

        public int GetMMDomainId(string name)
        {
            int id = 0;
            var mmDomain = _context.MMDomains.FirstOrDefault(c => c.MMDomain_Name == name);
            if (mmDomain == null)
                id = mmDomain.MMDomain_ID;

            return id;
        }

        public int GetSubMMDomainId(string name)
        {
            int id = 0;
            var mmSubDomain = _context.MMSubDomains.FirstOrDefault(c => c.MMSubDomain_Name == name);
            if (mmSubDomain == null)
                id = mmSubDomain.MMSubDomain_ID;

            return id;
        }

        public ScheduleViewModels GetMessageById(int id)
        {

            var result = new ScheduleViewModels();

            var triggerType = _context.ScheduleMessages.FirstOrDefault(c => c.ScheduleMessage_ID == id);

            if (triggerType.ScheduleMessage_TriggerTypeID != 7)
            {
                result = (from u in _context.ScheduleMessages.Where(c => c.ScheduleMessage_ID == id)
                          join trgType in _context.TriggerTypes on u.ScheduleMessage_TriggerTypeID equals trgType.TriggerType_ID into trgTypeL
                          from trgType in trgTypeL.DefaultIfEmpty()
                          join trgEvent in _context.TriggerEvents on u.ScheduleMessage_TriggerEventID equals trgEvent.TriggerEvent_ID into trgEventL
                          from trgEvent in trgEventL.DefaultIfEmpty()
                              // join trg in _context.Triggers on u.ScheduleMessage_TriggerID equals trg.Trigger_ID into trgL
                              // from trg in trgL.DefaultIfEmpty()
                          select new ScheduleViewModels
                          {
                              ScheduleMessageID = u.ScheduleMessage_ID,
                              Message = u.ScheduleMessage_Message,
                              If1 = u.ScheduleMessage_If1,
                              If2 = u.ScheduleMessage_If2,
                              TriggerTypeId = trgType.TriggerType_ID,
                              Time = u.ScheduleMessage_Time,
                              Day = u.Appointment_Schedule,
                              TriggerEventId = trgEvent.TriggerEvent_ID,
                              // TriggerId = trg.Trigger_ID

                          }).FirstOrDefault();

            }
            else
            {
                result = (from u in _context.ScheduleMessages.Where(c => c.ScheduleMessage_ID == id)
                          join trgType in _context.TriggerTypes on u.ScheduleMessage_TriggerTypeID equals trgType.TriggerType_ID into trgTypeL
                          from trgType in trgTypeL.DefaultIfEmpty()
                              //join trgEvent in _context.TriggerEvents on u.ScheduleMessage_TriggerEventID equals trgEvent.TriggerEvent_ID into trgEventL
                              //from trgEvent in trgEventL.DefaultIfEmpty()
                              // join trg in _context.Triggers on u.ScheduleMessage_TriggerID equals trg.Trigger_ID into trgL
                              // from trg in trgL.DefaultIfEmpty()
                          select new ScheduleViewModels
                          {
                              ScheduleMessageID = u.ScheduleMessage_ID,
                              Message = u.ScheduleMessage_Message,
                              If1 = u.ScheduleMessage_If1,
                              If2 = u.ScheduleMessage_If2,
                              TriggerTypeId = trgType.TriggerType_ID,
                              Time = u.ScheduleMessage_Time,
                              Day = u.Appointment_Schedule
                              // TriggerEventId = trgEvent.TriggerEvent_ID,
                              // TriggerId = trg.Trigger_ID

                          }).FirstOrDefault();
            }






            return result;
        }

        public EventViewModels GetTriggerEventById(int id)
        {
            var result = (from u in _context.TriggerEvents.Where(c => c.TriggerEvent_ID == id)
                          select new EventViewModels
                          {
                              Id = u.TriggerEvent_ID,
                              EventName = u.TriggerEvent_Event,
                              TriggerTypeId = u.TriggerTypeId,
                              // TriggerEvent_DateTime = u.TriggerEvent_DateTime,
                              TriggerEvent_Day = u.TriggerEvent_Day,
                          }).FirstOrDefault();
            return result;
        }

        public ProviderViewModel GetProviderById(int id)
        {
            var result = (from u in _context.Providers.Where(c => c.ProviderId == id)
                          select new ProviderViewModel
                          {
                              ProviderId = u.ProviderId,
                              ProviderName = u.ProviderName,
                              Address = u.Address,
                              ContactName = u.ContactName,
                              ContactEmail = u.ContactEmail,
                              ContactPhone = u.ContactPhone,
                              Phone = u.Phone
                          }).FirstOrDefault();
            return result;
        }
        public AppointmentViewModels GetAppointmentDetails(int id)
        {
            var result = (from ap in _context.Appointments.Where(c => c.Appointment_ID == id)
                          select new AppointmentViewModels
                          {

                              AppointmentID = ap.Appointment_ID,
                              UserId = ap.Appointment_PatientID,
                              ProviderId = ap.Appointment_ProviderID,
                              Description = ap.Appointment_Description,
                              StartTime = (DateTime)ap.Appointment_StartTime,
                              EndTime = ap.Appointment_EndTime,
                              Appointment_Trigger_EventId = (int)ap.Appointment_Trigger_EventId,
                              TypeName = _context.TriggerEvents.Where(x => x.TriggerEvent_ID == ap.Appointment_Trigger_EventId).Select(y => y.TriggerEvent_Event).FirstOrDefault(),
                          }).FirstOrDefault();

            return result;
        }

        //public List<AppintmentEvent> GetAllAppointmentEvents()
        //{
        //    return _context.AppintmentEvents.ToList();
        //}

        public List<DAL.TriggerEvent> GetAllAppointmentEvents()
        {
            return _context.TriggerEvents.Where(c => c.TriggerTypeId == (int)TriggerType_Enum.Appointment).ToList();
        }

        public Appointment GetAppointmentDetailsById(int id)
        {
            var result = _context.Appointments.FirstOrDefault(c => c.Appointment_ID == id);
            return result;
        }
        public User GetUserDetailsById(int id)
        {
            var result = _context.Users.FirstOrDefault(c => c.User_ID == id);
            return result;
        }

        public ScheduleMessage GetMessageDetailsById(int id)
        {
            var result = _context.ScheduleMessages.FirstOrDefault(c => c.ScheduleMessage_ID == id);
            return result;
        }

        public ScheduleMessage GetMessageByTriggerEventId(int eventId)
        {
            return _context.ScheduleMessages.FirstOrDefault(c => c.ScheduleMessage_TriggerEventID == eventId);
        }

        public void SaveMessageDetails(SmsReply sms)
        {
            _context.SmsReplies.Add(sms);
            _context.SaveChanges();
        }

        public DAL.TriggerEvent GetTriggerEventDetailsById(int id)
        {
            var result = _context.TriggerEvents.FirstOrDefault(c => c.TriggerEvent_ID == id);
            return result;
        }

        public Provider GetProviderDetailsById(int id)
        {
            var result = _context.Providers.FirstOrDefault(c => c.ProviderId == id);
            return result;
        }

        public List<DAL.Role> GetAllRoles()
        {
            List<DAL.Role> roles = _context.Roles.Where(c => c.Role_Name != "Admin").ToList();
            return roles;
        }

        public bool CheckUserNameExist(string userName)
        {
            var user = _context.Users.FirstOrDefault(c => c.User_Name == userName);
            if (user != null)
                return false;
            else
                return true;

        }

        public List<ProviderViewModel> GetAllProvider()
        {
            List<ProviderViewModel> providerList = new List<ProviderViewModel>();
            providerList = (from provider in _context.Providers
                            select new ProviderViewModel
                            {
                                ProviderId = provider.ProviderId,
                                ProviderName = provider.ProviderName,
                                Phone = provider.Phone,
                                Address = provider.Address,
                                ContactName = provider.ContactName,
                                ContactEmail = provider.ContactEmail,
                                ContactPhone = provider.ContactPhone
                            }).OrderByDescending(x => x.ProviderId).ToList();
            return providerList;
        }

        public List<EventViewModels> GetAllEvents()
        {
            List<EventViewModels> eventList = new List<EventViewModels>();
            eventList = (from ev in _context.TriggerEvents
                         join evType in _context.TriggerTypes on ev.TriggerTypeId equals evType.TriggerType_ID
                         where ev.TriggerEvent_Day != null
                         select new EventViewModels
                         {
                             Id = ev.TriggerEvent_ID,
                             EventName = ev.TriggerEvent_Event,
                             TriggerTypeName = evType.TriggerType_Type,
                             //  TriggerEvent_DateTime = ev.TriggerEvent_DateTime,
                             TriggerEvent_Day = ev.TriggerEvent_Day
                         }).OrderByDescending(c => c.Id).ToList();
            return eventList;
        }

        public List<DevinTwilio.Models.TriggerType> GetAllTriggerTypes()
        {
            var list = (from trgType in _context.TriggerTypes.Where(c => c.TriggerType_ID != (int)TriggerType_Enum.Response)
                        select new DevinTwilio.Models.TriggerType { TriggerTypeId = trgType.TriggerType_ID, TriggerTypeName = trgType.TriggerType_Type }).ToList();
            return list;
        }


        public DashoboardModel GetDashboardDetail()
        {
            var dashboard = (from ds in _context.DashboardTexts
                             select new DashoboardModel { Description = ds.Description, Id = ds.Id }).FirstOrDefault();

            return dashboard;
        }

        public void SaveDashboardDetail(DashoboardModel model)
        {
            DashboardText dashboard = new DashboardText { Id = model.Id, Description = model.EditorText };

            _context.DashboardTexts.Add(dashboard);
            _context.SaveChanges();
        }

        public void UpdateDashboardDetail(DashoboardModel model)
        {
            var dashboard = _context.DashboardTexts.FirstOrDefault(c => c.Id == model.Id);

            if (dashboard != null)
            {
                dashboard.Description = model.EditorText;
                _context.SaveChanges();
            }

        }


        public List<AppointmentViewModels> GetAllAppointments()
        {
            List<AppointmentViewModels> providerList = new List<AppointmentViewModels>();
            providerList = (from app in _context.Appointments
                            join user in _context.Users on app.Appointment_PatientID equals user.User_ID
                            join provider in _context.Providers on app.Appointment_ProviderID equals provider.ProviderId
                            join trgEvent in _context.TriggerEvents on app.Appointment_Trigger_EventId equals trgEvent.TriggerEvent_ID
                            select new AppointmentViewModels
                            {
                                UserId = user.User_ID,
                                AppointmentID = app.Appointment_ID,
                                ProviderId = provider.ProviderId,
                                ProviderName = provider.ProviderName,
                                UserName = user.User_Name,
                                StartTime = (DateTime?)app.Appointment_StartTime,
                                EndTime = app.Appointment_EndTime,
                                Description = app.Appointment_Description,
                                //TypeName = trgEvent.TriggerEvent_Event
                                TriggerEventDay = trgEvent.TriggerEvent_Day
                            }).OrderBy(c => c.StartTime).ToList();
            return providerList;
        }

        public List<ProviderViewModel> GetAllProviders()
        {
            List<ProviderViewModel> list = new List<ProviderViewModel>();
            list = (from pr in _context.Providers
                    select new ProviderViewModel { ProviderId = pr.ProviderId, ProviderName = pr.ProviderName, Address = pr.Address, ContactEmail = pr.ContactEmail, ContactName = pr.ContactName, Phone = pr.Phone }).ToList();
            return list;

        }

        public bool CheckEventAlredyExistOrNot(int? triggerTypeId, string eventName)
        {
            var result = _context.TriggerEvents.FirstOrDefault(c => c.TriggerTypeId == triggerTypeId && c.TriggerEvent_Day == eventName);

            if (result != null)
                return false;
            else
                return true;
        }

        public bool AddAppointment(Appointment model)
        {
            bool result = false;
            try
            {
                _context.Appointments.Add(model);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public bool AddEventType(DAL.TriggerEvent model)
        {
            bool result = false;
            try
            {
                _context.TriggerEvents.Add(model);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public bool AddUser(User model)
        {
            bool result = false;
            try
            {
                _context.Users.Add(model);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public List<DevinTwilio.DAL.TriggerEvent> GetAllEnrollmentEvents()
        {
            return _context.TriggerEvents.Where(c => c.TriggerTypeId == EnrollmentEventId).ToList();
        }
        public bool DeleteAppointment(int Id)
        {
            bool result = false;
            try
            {
                var appointments = _context.Appointments.FirstOrDefault(c => c.Appointment_ID == Id);
                _context.Appointments.Remove(appointments);
                _context.SaveChanges();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        public bool DeleteUser(int Id)
        {
            bool result = false;
            try
            {
                var users = _context.Users.FirstOrDefault(c => c.User_ID == Id);
                _context.Users.Remove(users);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public List<UserStatu> GetUserStatusList()
        {
            return _context.UserStatus.ToList();
        }

        public bool DeleteMessage(int Id)
        {
            bool result = false;
            try
            {
                var messages = _context.ScheduleMessages.FirstOrDefault(c => c.ScheduleMessage_ID == Id);
                _context.ScheduleMessages.Remove(messages);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public bool DeleteTriggerEvent(int Id)
        {
            bool result = false;
            try
            {
                var triggerevents = _context.TriggerEvents.FirstOrDefault(c => c.TriggerEvent_ID == Id);
                _context.TriggerEvents.Remove(triggerevents);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public bool DeleteProvider(int Id)
        {
            bool result = false;
            try
            {
                var providers = _context.Providers.FirstOrDefault(c => c.ProviderId == Id);
                _context.Providers.Remove(providers);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        public bool UpdateAppointment(Appointment model)
        {
            bool result = false;
            try
            {
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public bool UpdateUser(User model)
        {
            bool result = false;
            try
            {
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public bool UpdateMessage(ScheduleMessage model)
        {
            bool result = false;
            try
            {
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }


        public bool UpdateTriggerEvent(DAL.TriggerEvent model)
        {
            bool result = false;
            try
            {
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        public bool UpdateProvider(Provider model)
        {
            bool result = false;
            try
            {
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public bool AddProvider(Provider model)
        {
            bool result = false;
            try
            {
                _context.Providers.Add(model);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public UserViewModels GetPatientDetailById(int id)
        {
            var userList = (from user in _context.Users.Where(c => c.User_ID == id)
                            join status in _context.UserStatus on user.StatusId equals status.StatusId
                            select new UserViewModels
                            {
                                User_ID = user.User_ID,
                                User_Name = user.User_Name,
                                EmailAddress = user.EmailAddress,
                                User_PhoneNumber = user.User_PhoneNumber,
                                MobileNumber = user.MobileNumber,
                                EnrollmentDate = user.EnrollmentDate,
                                Notes = user.Notes,
                                UserStatus = status.Status
                            }).FirstOrDefault();
            return userList;
        }

        public List<AppointmentViewModels> GetAppointmentDetailById(int id)
        {
            List<AppointmentViewModels> appointmentList = new List<AppointmentViewModels>();
            appointmentList = (from appoint in _context.Appointments
                               join provider in _context.Providers on appoint.Appointment_ProviderID equals provider.ProviderId
                               join type in _context.TriggerEvents on appoint.Appointment_Trigger_EventId equals type.TriggerEvent_ID
                               where appoint.Appointment_PatientID == id
                               select new AppointmentViewModels
                               {
                                   TypeName = type.TriggerEvent_Event,
                                   ProviderName = provider.ProviderName,
                                   StartTime = (DateTime)appoint.Appointment_StartTime,
                                   EndTime = appoint.Appointment_EndTime
                               }).ToList();
            return appointmentList;
        }

        public List<SMSReplyViewModels> GetSentMessagesDetailById(int id)
        {
            List<SMSReplyViewModels> sentmessageList = new List<SMSReplyViewModels>();
            sentmessageList = (from sms in _context.SmsReplies
                               join sch in _context.ScheduleMessages on sms.MessageId equals sch.ScheduleMessage_ID
                               join ty in _context.TriggerEvents on sch.ScheduleMessage_TriggerEventID equals ty.TriggerEvent_ID
                               where sms.UserId == id
                               select new SMSReplyViewModels
                               {
                                   MessageId = (int)sms.MessageId,
                                   Message = sch.ScheduleMessage_Message,
                                   TypeName = ty.TriggerEvent_Event,  //_context.TriggerTypes.Where(x => x.TriggerType_ID == sms.MessageId).Select(x => x.TriggerType_Type).FirstOrDefault(),
                                   SmsStatus = sms.SmsStatus,
                                   MessageResponse = sms.MessageResponse,
                                   SentDate = sms.SentDate
                               }).ToList();
            return sentmessageList;
        }

        public List<SMSReplyViewModels> GetSentMessagesDetailByMessageId(int id)
        {
            List<SMSReplyViewModels> sentmessageList = new List<SMSReplyViewModels>();
            sentmessageList = (from sms in _context.SmsReplies
                               join sch in _context.ScheduleMessages on sms.MessageId equals sch.ScheduleMessage_ID
                               join ty in _context.TriggerEvents on sch.ScheduleMessage_TriggerEventID equals ty.TriggerEvent_ID
                               where sms.MessageId == id
                               select new SMSReplyViewModels
                               {
                                   MessageId = (int)sms.MessageId,
                                   Message = sch.ScheduleMessage_Message,
                                   TypeName = ty.TriggerEvent_Event,  //_context.TriggerTypes.Where(x => x.TriggerType_ID == sms.MessageId).Select(x => x.TriggerType_Type).FirstOrDefault(),
                                   SmsStatus = sms.SmsStatus,
                                   MessageResponse = sms.MessageResponse,
                                   SentDate = sms.SentDate,
                                   To = sms.To

                               }).ToList();
            return sentmessageList;
        }



        //public List<EnrollmentDetail> GetNextMessagesEnrollmentDetailById(int id)
        //{
        //    List<EnrollmentDetail> sentmessageEnrollmentList = new List<EnrollmentDetail>();
        //    sentmessageEnrollmentList = (from user in _context.Users.Where(c => c.User_Name != "admin")
        //                                 join appoint in _context.Appointments on user.User_ID equals appoint.Appointment_PatientID
        //                                 join smsreply in _context.SmsReplies on user.User_ID equals smsreply.UserId
        //                                 join sch in _context.ScheduleMessages on smsreply.MessageId equals sch.ScheduleMessage_ID
        //                                 where user.User_ID == id 
        //                                 orderby sch.ScheduleMessage_TriggerEventID ascending
        //                                 select new EnrollmentDetail
        //                                 {
        //                                     User_ID = user.User_ID,
        //                                     User_Name = user.User_Name,
        //                                     MessageID = (int)smsreply.MessageId,
        //                                     Message = sch.ScheduleMessage_Message,
        //                                     EnrollmentDate = user.EnrollmentDate,
        //                                //   NextAppointment = (DateTime)_context.Appointments.Where(x => x.Appointment_StartTime > currentDate).Select(x => x.Appointment_StartTime).FirstOrDefault(),
        //                                     Status = smsreply.SmsStatus,
        //                                     NextAppointment = user.EnrollmentDate,
        //                                     TriggerEventId=sch.ScheduleMessage_TriggerEventID,
        //                                 }).OrderByDescending(m=>m.TriggerEventId).ToList();

        //    //sentmessageEnrollmentList = sentmessageEnrollmentList.Where(m => m.EnrollmentDate <= m.EnrollmentDate.Value.AddDays(50) && m.EnrollmentDate >= m.EnrollmentDate).ToList();
        //    //var enrollResult = sentmessageEnrollmentList.GroupBy(c => new { c.MessageID })
        //    // .Select(s => s.First());

        //    var enrollmentList = _context.TriggerEvents.Where(c => c.TriggerEvent_Event.Contains("Day")).Select(m => m.TriggerEvent_ID).ToList();
        //    var enrollmentFinalList = (from member in sentmessageEnrollmentList
        //                          where enrollmentList.Contains(member.TriggerEventId.Value) select member).ToList();
        //    return enrollmentFinalList;
        //}

        public List<EnrollmentDetail> GetNextMessagesEnrollmentDetailById(int id)
        {
            var enrollmentList = _context.TriggerEvents.Where(c => c.TriggerTypeId == (int)TriggerType_Enum.Enrollment).Select(m => m.TriggerEvent_Day).ToList();
            var userDetails = _context.Users.FirstOrDefault(c => c.User_ID == id);
            var scheduledMessageList = (from msg in _context.ScheduleMessages.Where(c => c.ScheduleMessage_TriggerTypeID != (int)TriggerType_Enum.Response)
                                        join trgEvent in _context.TriggerEvents on
                                        msg.ScheduleMessage_TriggerEventID equals trgEvent.TriggerEvent_ID
                                        select new EnrollmentDetail
                                        {
                                            Message = msg.ScheduleMessage_Message,
                                            TriggerEventId = trgEvent.TriggerEvent_ID,
                                            // ScheduledName = trgEvent.TriggerEvent_Event,
                                            TriggerEventNum = trgEvent.TriggerEvent_Day,
                                            MessageID = msg.ScheduleMessage_ID,
                                            User_ID = id,
                                            Time = msg.ScheduleMessage_Time
                                        }).OrderBy(c => c.TriggerEventNum).ToList();

            //  var distinctScheduledMessageList = scheduledMessageList.Where(c => c.ScheduledName.Contains("Day")).Distinct().ToList();



            List<EnrollmentDetail> newList = new List<EnrollmentDetail>();

            foreach (var item in scheduledMessageList)
            {
                if (newList.FirstOrDefault(c => c.TriggerEventNum == item.TriggerEventNum) == null)
                {
                    newList.Add(item);
                }
            }

            var userEnrollmentDate = userDetails.EnrollmentDate;
            List<EnrollmentDetail> list = new List<EnrollmentDetail>();

            if (userEnrollmentDate != null)
            {
                foreach (var item in enrollmentList)
                {
                    EnrollmentDetail enrollment = new EnrollmentDetail();
                    var messageDetails = newList.FirstOrDefault(c => c.TriggerEventNum == item);
                    if (messageDetails != null)
                    {
                        //var scheduledName = item.Split(' ');
                        //if (scheduledName.Length > 0)
                        //{
                        //    var dayNum = scheduledName[1];
                        int day;
                        if (int.TryParse(item.ToString(), out day))
                        {
                            enrollment.Date = userEnrollmentDate.Value.AddDays(day);
                            enrollment.EnrollmentMessageDate = enrollment.Date.Value.ToShortDateString() + " " + messageDetails.Time;
                        }
                        //}
                        enrollment.TriggerEventNum = item;
                        enrollment.Message = messageDetails.Message;
                        enrollment.MessageID = messageDetails.MessageID;
                        enrollment.TriggerEventId = messageDetails.TriggerEventId;
                        enrollment.User_ID = id;
                        list.Add(enrollment);
                    }
                }
            }

            list = list.Where(c => c.Date > DateTime.Now).OrderBy(c => c.Date).ToList();

            return list;
        }

        public List<AppointmentDetail> GetNextMessagesAppointmentDetailById(int id)
        {
            List<AppointmentDetail> sentmessageAppointmentList = new List<AppointmentDetail>();
            var currentDate = System.DateTime.Now;
            sentmessageAppointmentList = (from appoint in _context.Appointments.Where(c => c.Appointment_PatientID == id)
                                          join sch in _context.ScheduleMessages on appoint.Appointment_Trigger_EventId equals sch.ScheduleMessage_TriggerEventID
                                          join evt in _context.TriggerEvents on appoint.Appointment_Trigger_EventId equals evt.TriggerEvent_ID
                                          join user in _context.Users on appoint.Appointment_PatientID equals user.User_ID
                                          select new AppointmentDetail
                                          {
                                              MessageID = sch.ScheduleMessage_ID,
                                              Message = sch.ScheduleMessage_Message,
                                              AppointmentDate = appoint.Appointment_StartTime,
                                              TriggerEventId = sch.ScheduleMessage_TriggerEventID,
                                              Time = sch.ScheduleMessage_Time,
                                              AppoinmentScheduleDay = sch.Appointment_Schedule,
                                              User_ID = user.User_ID,


                                          }).ToList();
            //    sentmessageAppointmentList = sentmessageAppointmentList.Where(m => m.AppointmentDate.Value.AddDays(-1).Date == currentDate.Date || m.AppointmentDate.Value.AddDays(-7).Date == currentDate.Date).ToList();

            sentmessageAppointmentList = sentmessageAppointmentList.Where(m => m.AppointmentDate.Value > currentDate.AddDays(-10)).ToList();

            var appointResult = sentmessageAppointmentList.GroupBy(c => new { c.MessageID })
               .Select(s => s.First());

            var newAppoinmentList = new List<AppointmentDetail>();

            foreach (var item in appointResult)
            {

                AppointmentDetail app = new AppointmentDetail();
                app.MessageID = item.MessageID;
                app.Message = item.Message;
                app.User_ID = item.User_ID;
                app.TriggerEventId = item.TriggerEventId;
                if (item.AppoinmentScheduleDay != null)
                {
                    var finalAppoinmentDate = item.AppointmentDate.Value.AddDays(item.AppoinmentScheduleDay.Value);
                    app.AppoinmentMessageDeliveryDateTime = finalAppoinmentDate.ToShortDateString() + " " + item.Time;
                }

                newAppoinmentList.Add(app);

            }

            // var finalAppoinmentDate = item.AppointmentDate.Value.AddDays(item.Day.Value);


            return newAppoinmentList.ToList();

            // return appointResult.ToList();
        }

    }
}