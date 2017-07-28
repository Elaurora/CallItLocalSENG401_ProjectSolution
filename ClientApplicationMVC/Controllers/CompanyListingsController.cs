using ClientApplicationMVC.Models;

using Messages.DataTypes.Database.CompanyDirectory;

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

            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }
            CompanyList result = ServiceBusCommunicationManager.searchCompanyByName(textCompanyName);
            if (result == null)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ViewBag.Companylist = result;

            return View("Index");
        }

        public ActionResult DisplayCompany(string info)
        {
            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }
            if ("".Equals(info))
            {
                return View("Index");
            }
            CompanyInstance company = ServiceBusCommunicationManager.getCompanyInfo(info);

            ViewBag.CompanyInfo = company;

            ViewBag.CompanyName = info;
            return View("DisplayCompany");
        }
    }
}