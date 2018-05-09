using DevinTwilio.DbLayer;
using DevinTwilio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevinTwilio.DbLayer;
using DevinTwilio.Common;

namespace DevinTwilio.Controllers
{
    public class TriggerEventController : Controller
    {
        // GET: TriggerEvent
        UserService userService = new UserService();
        public ActionResult Index()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            UserService _context = new UserService();
            return View(_context.GetAllEvents());
        }




        public ActionResult Create()
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
        public ActionResult Create(EventViewModels model)
        {
            if (model.TriggerTypeId == (int)TriggerType_Enum.Appointment)
            {
                ModelState.Remove("TriggerEvent_Day");
                model.TriggerEvent_Day = model.EventName;
            }
            else if (model.TriggerTypeId == (int)TriggerType_Enum.Enrollment)
            {
                ModelState.Remove("EventName");
                model.TriggerEvent_Day = model.TriggerEvent_Day;

                int n;
                var isNumeric = int.TryParse(model.TriggerEvent_Day, out n);

                if (!isNumeric)
                {
                    TempData["ErrorMessage"] = "Only number allow for day field of enrollment type";
                    model.TriggerTypeList = userService.GetAllTriggerTypes();
                    return View(model);
                }



            }

            if (ModelState.IsValid)
            {

                if (!userService.CheckEventAlredyExistOrNot(model.TriggerTypeId, model.TriggerEvent_Day))
                {
                    TempData["ErrorMessage"] = "Event alredy exist, please try another day number.";
                    model.TriggerTypeList = userService.GetAllTriggerTypes();
                    return View(model);
                }


                DAL.TriggerEvent eventType = new DAL.TriggerEvent
                {
                    TriggerEvent_Event = model.EventName,
                    TriggerTypeId = model.TriggerTypeId,
                    TriggerEvent_Day = model.TriggerEvent_Day,
                    // TriggerEvent_DateTime = model.TriggerEvent_DateTime

                };
                if (userService.AddEventType(eventType))
                    return RedirectToAction("index");

            }
            model.TriggerTypeList = userService.GetAllTriggerTypes();
            return View(model);
        }

        public ActionResult Edit(int id = 0)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            EventViewModels viewModel = new EventViewModels();
            if (id != 0)
            {
                viewModel = userService.GetTriggerEventById(id);

                //if(viewModel.TriggerEvent_DateTime!=null)
                //{
                //    viewModel.StrTriggerEvent_DateTime = viewModel.TriggerEvent_DateTime.Value.ToString("MM/dd/yyyy HH:mm tt");
                //}

            }
            viewModel.TriggerTypeList = userService.GetAllTriggerTypes();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EventViewModels model)
        {
            if (model.TriggerTypeId == (int)TriggerType_Enum.Appointment)
            {
                ModelState.Remove("TriggerEvent_Day");
                model.TriggerEvent_Day = model.EventName;
            }
            else if (model.TriggerTypeId == (int)TriggerType_Enum.Enrollment)
            {
                ModelState.Remove("EventName");
                model.TriggerEvent_Day = model.TriggerEvent_Day;

                int n;
                var isNumeric = int.TryParse(model.TriggerEvent_Day, out n);

                if (!isNumeric)
                {
                    TempData["ErrorMessage"] = "Only number allow for day field of enrollment type";
                    model.TriggerTypeList = userService.GetAllTriggerTypes();
                    return View(model);
                }


            }
            //EventViewModels viewModel = new EventViewModels();

            if (ModelState.IsValid)
            {


                //if (model.StrTriggerEvent_DateTime != null)
                //{
                //    ModelState.Remove("StartTime");
                //    model.TriggerEvent_DateTime = Convert.ToDateTime(model.StrTriggerEvent_DateTime);
                //}




                if (model.Id != 0)
                {
                    var dbResult = userService.GetTriggerEventDetailsById(model.Id);
                    dbResult.TriggerEvent_Day = model.TriggerEvent_Day;
                    //   dbResult.TriggerEvent_DateTime = model.TriggerEvent_DateTime;
                    if (userService.UpdateTriggerEvent(dbResult))
                        return RedirectToAction("index");
                }

            }

            return View(model);
        }

        public ActionResult Delete(int id = 0)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            userService.DeleteTriggerEvent(id);
            return RedirectToAction("index");
        }




    }
}