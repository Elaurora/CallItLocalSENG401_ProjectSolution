using ClientApplicationMVC.Models;

using Messages.Message;

using System;
using System.Web.Mvc;

namespace ClientApplicationMVC.Controllers
{
    public class CompanyListingsController : Controller
    {
        // GET: CompanyListings
        public ActionResult Index()
        {
            if (Globals.isLoggedIn())
            {
                ViewBag.Companylist = null;
                return View("Index");
            }
            return RedirectToAction("Index", "Authentication");
        }

        public ActionResult Search(string textCompanyName)
        {

            if(Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }
            CompanyList result = ServiceBusCommunicationManager.searchCompanyByName(textCompanyName);
            if(result == null)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ViewBag.Companylist = result;

            return View("Index");
        }
    }
}