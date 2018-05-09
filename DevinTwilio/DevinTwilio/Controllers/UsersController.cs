using DevinTwilio.DAL;
using DevinTwilio.DbLayer;
using DevinTwilio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace DevinTwilio.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        UserService _service = new UserService();

        public ActionResult Index()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            UserService userService = new UserService();
            return View(userService.GetAllPatients());
        }

        public ActionResult Users()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            UserService userService = new UserService();
            return View(userService.GetAllUsers());
        }

        public ActionResult Logs()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            UserViewModels viewModel = new UserViewModels();
            viewModel.UsesList = _service.GetAllUsers();
            return View(viewModel);
        }
        public PartialViewResult _LogsDetailByPatientPartialView(string patientname, int userId)
        {
            UserService _context = new UserService();
            var list = _context.GetAllLogsByPatient(patientname);
            ViewBag.Patientname = patientname;
            ViewBag.PatientId = userId;
            return PartialView(list);
        }
        // GET: Users/Details/5
        public ActionResult Details(int id)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.ID = id;
            return View();
        }

        // GET: Users/Create
        public ActionResult Register()
        {
            UserViewModels model = new UserViewModels();
            model.Roles = _service.GetAllRoles();
            return View(model);
        }

        // POST: Users/Create
        [HttpPost]
        public ActionResult Register(UserViewModels model, string returnurl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!_service.CheckUserNameExist(model.User_Name))
                    {
                        model.Roles = _service.GetAllRoles();
                        TempData["ErrorMessage"] = "User name already exist, please try another.";
                        return View(model);
                    }

                    string mobileNumWithCountryCode = "+1" + model.MobileNumber;

                    User user = new User
                    {
                        EmailAddress = model.EmailAddress,
                        User_Name = model.User_Name,
                        //  Role_Id = model.Role_Id,
                        Password = model.Password,
                        MobileNumber = mobileNumWithCountryCode,
                        Notes = model.Notes,
                        EnrollmentDate = model.EnrollmentDate,
                        CreatedDate = System.DateTime.Now,
                        StatusId=1 // default Active
                        
                    };
                    _service.AddUser(user);

                    var enrollmentList = _service.GetAllEnrollmentEvents();
         
                
                    model.Roles = _service.GetAllRoles();
                    TempData["successMsg"] = "You have been sucessfully registred.";

                    return RedirectToAction("Users", "Users"); //Request.UrlReferrer.ToString()
                                                               //  return RedirectToAction(Request.UrlReferrer.ToString(), "Users");
                }
                else
                {
                    model.Roles = _service.GetAllRoles();
                    return View(model);
                }
            }
            catch
            {
                return View();
            }
        }

        // Patient Profile Partial Views
        public ActionResult _PatientDetailPartialView(int id)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            UserService _context = new UserService();
            return PartialView(_context.GetPatientDetailById(id));
        }
        public ActionResult _AppointmentPatientDetailPartialView(int id)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            UserService _context = new UserService();
            return PartialView(_context.GetAppointmentDetailById(id));
        }
        public ActionResult _SentMessagesPatientDetailPartialView(int id)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            UserService _context = new UserService();
            return View(_context.GetSentMessagesDetailById(id));
        }
        public ActionResult _NextMessagesPatientDetailPartialView(int id)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            UserService _context = new UserService();
            return View(_context.GetNextMessagesEnrollmentDetailById(id));
        }

        public ActionResult _NextMessagesPatientAppointmentDetailPartialView(int id)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            UserService _context = new UserService();
            return View(_context.GetNextMessagesAppointmentDetailById(id));
        }

        public ActionResult AddOrEdit(int id = 0, string actionDo = "")
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            UserViewModels viewModel = new UserViewModels();
            if (id != 0)
            {
                viewModel = _service.GetUserById(id);
                var mobileNum = "";
                if (viewModel.MobileNumber.Contains("+1"))
                    mobileNum = viewModel.MobileNumber.Replace("+1", "");
                else
                    mobileNum = viewModel.MobileNumber.Replace("+91", "");

                viewModel.MobileNumber = mobileNum;

            }
            if (viewModel.EnrollmentDate != null)
                viewModel.strEnrollmentDate = viewModel.EnrollmentDate.Value.ToString("MM/dd/yyyy HH:mm tt");

            viewModel.UsesList = _service.GetAllUsers();
            viewModel.UserStatusList = _service.GetUserStatusList();
            viewModel.ActionToDo = actionDo;


            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddOrEdit(UserViewModels model)
        {
            UserViewModels viewModel = new UserViewModels();

            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            viewModel.UsesList = _service.GetAllUsers();
            viewModel.UserStatusList = _service.GetUserStatusList();

            if (model.strEnrollmentDate != null)
            {
                model.EnrollmentDate = Convert.ToDateTime(model.strEnrollmentDate);
            }

            //if (ModelState.IsValid)
            //{

            if (model.User_ID != 0)
            {
                var dbResult = _service.GetUserDetailsById(model.User_ID);
                dbResult.User_Name = model.User_Name;
                dbResult.MobileNumber = "+1" + model.MobileNumber;
                dbResult.EmailAddress = model.EmailAddress;
                dbResult.CreatedDate = System.DateTime.Now;
                dbResult.EnrollmentDate = model.EnrollmentDate;
                dbResult.StatusId = model.StatusId;
                dbResult.Notes = model.Notes;
                if (_service.UpdateUser(dbResult))
                {
                    if(model.ActionToDo== "Details")
                    {
                        return RedirectToAction("Details", "Users", new { id = model.User_ID });
                    }
                    else
                    {
                        return RedirectToAction("Users");
                    }
                }
                    
            }

            //}

            return View(viewModel);
        }

        public ActionResult Delete(int id = 0)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            _service.DeleteUser(id);
            return RedirectToAction("Users");
        }

        [HttpGet]
        public ActionResult UpdateStatus(int userId=0)
        {
            UserViewModels model = new UserViewModels();
          
            model = _service.GetUserById( Convert.ToInt32(userId));
            model.UserStatusList =_service.GetUserStatusList();

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateStatus(UserViewModels model)
        {
            model.UserStatusList = _service.GetUserStatusList();

            var dbUser = _service.GetUserDetailById(model.User_ID);
            dbUser.StatusId = model.StatusId;
            _service.UpdateUser(dbUser);
            return RedirectToAction("Details", "Users", new { id = model.User_ID });
        }

        public ActionResult AddUserEnrollment(int userId=0, int enrollmentId=0, string actionToDo = "")
        {
            _service.AddUserEnrollment(userId, enrollmentId, actionToDo);

            if(actionToDo== "Send")
            {
                try
                {
                    const string accountSid = "ACb61b5dc2ad53c6336667449e67302c48";
                    const string authToken = "b1dfd0ebd09b0bd2d840f07618b33a27";
                    TwilioClient.Init(accountSid, authToken);

                    var userDetail = _service.GetUserById(userId);
                    string _toNumber = userDetail.MobileNumber;
                    var _fromNumber = new PhoneNumber("+16467591379");

                    string messageBody = "";
                    var messageDetails = _service.GetMessageByTriggerEventId(enrollmentId);
                    if (messageDetails != null)
                    {
                        messageBody = messageDetails.ScheduleMessage_Message;
                    }
                    var message = MessageResource.Create(to: _toNumber, from: _fromNumber, body: messageBody);
                    string Sid = Convert.ToString(message.Sid);

                    if (!string.IsNullOrEmpty(Sid))
                    {


                        SmsReply _smsReply = new SmsReply();
                        _smsReply.From = Convert.ToString(_fromNumber);
                        _smsReply.To = Convert.ToString(_toNumber);
                        _smsReply.Sid = Convert.ToString(Sid);
                        _smsReply.ParentSid = null;
                        _smsReply.MessageId = messageDetails.ScheduleMessage_ID;
                        _smsReply.UserId = userId;
                        _smsReply.SentDate = DateTime.Now;
                        _service.SaveMessageDetails(_smsReply);

                    }
                }
                catch(Exception ex)
                {

                }

               



            }
            return RedirectToAction("Details", "Users", new { id = userId });
        }

        [HttpPost]
        public ActionResult UpdateUserEnrollment(UserViewModels model)
        {
            return View();
        }



    }
}
