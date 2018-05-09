using DevinTwilio.DAL;
using DevinTwilio.DbLayer;
using DevinTwilio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DevinTwilio.Controllers
{
    public class AppointmentController : Controller
    {
        // GET: Appointment

        UserService userService = new UserService();
        public ActionResult Index()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(userService.GetAllAppointments());
        }


        public ActionResult Create()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            AppointmentViewModels viewModel = new AppointmentViewModels();
            viewModel.UsesList = userService.GetAllUsers();
            viewModel.ProviderList = userService.GetAllProvider();
            viewModel.AppointmentEventList = userService.GetAllAppointmentEvents();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(AppointmentViewModels model)
        {

            if (ModelState.IsValid)
            {
                //if (!userService.CheckAppointmentExistForPatientOrNot(model.UserId))
                //{
                //    model.UsesList = userService.GetAllUsers();
                //    model.ProviderList = userService.GetAllProvider();
                //    model.AppointmentEventList = userService.GetAllAppointmentEvents();

                //    TempData["ErrorMessage"] = "Apointment already exist for this patient, you can edit.";
                //    return View(model);
                //}

                var appointMentdate = Convert.ToDateTime(model.StartTime);


                Appointment app = new Appointment
                {
                    Appointment_PatientID = model.UserId,
                    Appointment_Description = model.Description,
                    Appointment_ProviderID = model.ProviderId,
                    Appointment_StartTime = model.StartTime,
                    Appointment_EndTime = model.EndTime,
                    Appointment_Trigger_EventId = model.Appointment_Trigger_EventId
                };
                if (userService.AddAppointment(app))
                    return RedirectToAction("Index");

            }


            model.UsesList = userService.GetAllUsers();
            model.ProviderList = userService.GetAllProvider();
            model.AppointmentEventList = userService.GetAllAppointmentEvents();
            return View(model);
        }


        public ActionResult AddOrEdit(int id = 0)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            AppointmentViewModels viewModel = new AppointmentViewModels();
            if (id != 0)
            {
                viewModel = userService.GetAppointmentDetails(id);
            }

            //viewModel.StrStartTime = viewModel.StartTime.Value.ToString("MM/dd/yyyy HH:mm tt");
            viewModel.StrStartTime = viewModel.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss");

            //viewModel.StrStartTime = "02/06/2018 01:02 PM";

            // viewModel.StartTime = DateTime.Now;

            viewModel.UsesList = userService.GetAllUsers();
            viewModel.ProviderList = userService.GetAllProvider();
            viewModel.AppointmentEventList = userService.GetAllAppointmentEvents();

            return View(viewModel);
        }


        [HttpPost]
        public ActionResult AddOrEdit(AppointmentViewModels model)
        {
            // AppointmentViewModels viewModel = new AppointmentViewModels();

            model.UsesList = userService.GetAllUsers();
            model.ProviderList = userService.GetAllProvider();
            model.AppointmentEventList = userService.GetAllAppointmentEvents();

            if(model.StrStartTime != null)
            {
                ModelState.Remove("StartTime");
                model.StartTime = Convert.ToDateTime(model.StrStartTime);
            }
              

            if(!ModelState.IsValid)
            {

                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        string res = "";
                    }
                }


                return View(model);
            }

            try
            {
                if (model.AppointmentID != 0)
                {
                    var dbResult = userService.GetAppointmentDetailsById(model.AppointmentID);
                    dbResult.Appointment_PatientID = model.UserId;
                    dbResult.Appointment_ProviderID = model.ProviderId;
                    dbResult.Appointment_StartTime = model.StartTime;
                    dbResult.Appointment_EndTime = model.EndTime;
                    dbResult.Appointment_Description = model.Description;
                    dbResult.Appointment_PatientID = model.UserId;
                    dbResult.Appointment_Trigger_EventId = model.Appointment_Trigger_EventId;

                    if (userService.UpdateAppointment(dbResult))
                        return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
           

            //else
            //{
            //    if (!userService.CheckAppointmentExistForPatientOrNot(model.UserId))
            //    {
            //        TempData["ErrorMessage"] = "Apointment already exist for this patient, you can edit.";
            //        return View(model);
            //    }


            //    var appointMentdate = Convert.ToDateTime(model.StartTime);

            //    Appointment app = new Appointment { Appointment_PatientID = model.UserId, Appointment_ProviderID = model.ProviderId, Appointment_StartTime = model.StartTime, Appointment_EndTime = model.EndTime, Appointment_Trigger_EventId = model.Appointment_Trigger_EventId };
            //    if (userService.AddAppointment(app))
            //        return RedirectToAction("Index");
            //}
            return View(model);
        }

        public ActionResult Delete(int id = 0)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            userService.DeleteAppointment(id);
            return RedirectToAction("Index");
        }

    }
}