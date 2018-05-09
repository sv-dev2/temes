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
    public class ProviderController : Controller
    {
        // GET: Provider

        UserService userService = new UserService();
        public ActionResult Index()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(userService.GetAllProvider());
        }

        public ActionResult Create()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProviderViewModel model)
        {
            if (ModelState.IsValid)
            {
                Provider pr = new Provider { ProviderName = model.ProviderName, Address = model.Address, ContactEmail = model.ContactEmail, ContactName = model.ContactName, Phone = model.Phone };
                if (userService.AddProvider(pr))
                    return RedirectToAction("index");
            }
            return View(model);
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ProviderViewModel viewModel = new ProviderViewModel();
            if (id != 0)
            {
                viewModel = userService.GetProviderById(id);
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddOrEdit(ProviderViewModel model)
        {
            ProviderViewModel viewModel = new ProviderViewModel();
            if (model.ProviderId != 0)
            {
                var dbResult = userService.GetProviderDetailsById(model.ProviderId);
                dbResult.ProviderName = model.ProviderName;
                dbResult.ContactPhone = model.ContactPhone;
                dbResult.Address = model.Address;
                dbResult.ContactName = model.ContactName;
                dbResult.ContactEmail = model.ContactEmail;
                dbResult.Phone = model.Phone;

                //if (!userService.CheckProviderNameExistOrNot(model.ProviderName))
                //{
                //    TempData["ErrorMessage"] = "ProviderName already exist, you can edit.";
                //    return View(viewModel);
                //}
                //else
                //{
                    if (userService.UpdateProvider(dbResult))
                        return RedirectToAction("Index");
               // }

            }
           
            return View(viewModel);
        }

        public ActionResult Delete(int id = 0)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            userService.DeleteProvider(id);
            return RedirectToAction("Index");
        }
    }
}