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
            Socket connection = new Socket(ServiceBusInfo.ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            connection.Connect("localhost", 11000);

            textUsername += "<EOF>";
            byte[] msg = Encoding.ASCII.GetBytes(textUsername);
            connection.Send(msg);

            textPassword += "<EOF>";
            msg = Encoding.ASCII.GetBytes(textPassword);
            connection.Send(msg);

            string response = readUntilEOF(connection);

            ViewBag.Title = "AuthenticationSuccess";
            ViewBag.Result = response;
            return View("AuthenticationSuccess");
        }

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