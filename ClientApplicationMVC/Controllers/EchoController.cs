using ClientApplicationMVC.Models;

using Messages.ServiceBusRequest.Echo.Requests;
using Messages.ServiceBusRequest;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientApplicationMVC.Controllers
{
    public class EchoController : Controller
    {
        // GET: Echo
        public ActionResult Index()
        {
            ViewBag.AsIsResponse = " ";
            ViewBag.ReverseResponse = " ";
            return View();
        }

        public ActionResult AsIsEcho(string asIsText)
        {
            AsIsEchoRequest request = new AsIsEchoRequest(asIsText);
            ServiceBusResponse response;
            ServiceBusConnection connection = ConnectionManager.getConnectionObject(Globals.getUser());
            if(connection == null)
            {
                response = ConnectionManager.echoAsIs(request);
            }
            else
            {
                response = connection.echoAsIs(request);
            }

            ViewBag.AsIsResponse = response.response;

            return View("Index");
        }

        public ActionResult ReverseEcho(string reverseText)
        {
            ReverseEchoRequest request = new ReverseEchoRequest(reverseText, Globals.getUser());
            ServiceBusResponse response;
            ServiceBusConnection connection = ConnectionManager.getConnectionObject(Globals.getUser());
            if (connection == null)
            {
                response = new ServiceBusResponse(false, "Error: You must be logged in to use the echo reverse functionality.");
            }
            else
            {
                response = connection.echoReverse(request);
            }

            ViewBag.ReverseResponse = response.response;

            return View("Index");
        }
    }
}