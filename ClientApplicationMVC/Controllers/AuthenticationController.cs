using ClientApplicationMVC.Models;

using System;
using System.Net.Sockets;
using System.Text;
using System.Web.Mvc;

namespace ClientApplicationMVC.Controllers
{
    public class AuthenticationController : Controller
    {
        
        /// <summary>
        /// This is the controller used when the user first navigates to the login page before they have entered any information
        /// </summary>
        /// <returns>The view used for entering login information</returns>
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Authentication";
            return View();
        }

        /// <summary>
        /// This is the controller that is used once the user has entered login information
        /// </summary>
        /// <param name="textUsername">The username entered into the textField</param>
        /// <param name="textPassword">The password entered into the textField</param>
        /// <returns>The new view to be displayed</returns>
        [HttpPost]
        [AsyncTimeout(50000)]
        public ActionResult Index(string textUsername, string textPassword)
        {
            string response = ServiceBusConnection.sendCredentials(textUsername, textPassword);

            ViewBag.Title = "AuthenticationSuccess";
            ViewBag.Result = response;
            return RedirectToAction("Index", "Home");
            return View("~/Views/Home/Index.aspx");
        }

        /// <summary>
        /// Continuously reads one byte at a time from the client until the "<EOF>" string of characters is found
        /// </summary>
        /// <returns>The string representation of bytes read from the server socket</returns>
        private string readUntilEOF(Socket connection)
        {
            byte[] readByte = new byte[1];
            string returned = String.Empty;

            while (returned.Contains("<EOF>") == false)
            {
                connection.Receive(readByte, 1, 0);
                returned += (char)readByte[0];
            }

            return returned.Substring(0, returned.IndexOf("<EOF>"));
        }
    }
}