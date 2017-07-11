using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientApplicationMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //TODO: Add functionality to search for companies.
        //TODO: Create an emailing service.
        //TODO: Move Console.Write calls to a single function
        //TODO: Make a "ServiceStart" project to be run from cmd line to start the services when deployed
    }
}