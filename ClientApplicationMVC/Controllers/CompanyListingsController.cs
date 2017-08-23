using ClientApplicationMVC.Models;

using Messages.DataTypes.Database.CompanyDirectory;

using System;
using System.Web.Mvc;

namespace ClientApplicationMVC.Controllers
{
    /// <summary>
    /// This class contains the functions responsible for handling requests routed to *Hostname*/CompanyListings/*
    /// </summary>
    public class CompanyListingsController : Controller
    {
        /// <summary>
        /// This function is called when the client navigates to *hostname*/CompanyListings
        /// </summary>
        /// <returns>A view to be sent to the client</returns>
        public ActionResult Index()
        {
            if (Globals.isLoggedIn())
            {
                ViewBag.Companylist = null;
                return View("Index");
            }
            return RedirectToAction("Index", "Authentication");
        }

        /// <summary>
        /// This function is called when the client navigates to *hostname*/CompanyListings/Search
        /// </summary>
        /// <returns>A view to be sent to the client</returns>
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

        /// <summary>
        /// This function is called when the client navigates to *hostname*/CompanyListings/DisplayCompany/*info*
        /// </summary>
        /// <param name="info">The name of the company whos info is to be displayed</param>
        /// <returns>A view to be sent to the client</returns>
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