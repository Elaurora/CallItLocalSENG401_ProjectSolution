using ClientApplicationMVC.Models;

using Messages.Commands;
using Messages.DataTypes;

using System;
using System.Net.Sockets;
using System.Text;
using System.Web.Mvc;

namespace ClientApplicationMVC.Controllers
{
    public class AuthenticationController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Please enter your username and password.";
            return View("Index");
        }

        /// <summary>
        /// This is the controller that is used once the user has entered login information
        /// </summary>
        /// <param name="textUsername">The username entered into the textField</param>
        /// <param name="textPassword">The password entered into the textField</param>
        /// <returns>The new view to be displayed</returns>
        [HttpPost]
        [AsyncTimeout(Globals.patienceLevel_ms)]
        public ActionResult LogIn(string textUsername, string textPassword)
        {
            string response = ServiceBusCommunicationManager.sendLogIn(textUsername, textPassword);

            
            //TODO: React based on response

            if ("Success".Equals(response))
            {
                ViewBag.Title = "Authentication Success";
                Globals.setUser(textUsername);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Title = "Authentication Failure";
            ViewBag.Message = "Incorrect Username or Password. Please try again";

            return View("Index");
        }

        [HttpGet]
        public ActionResult CreateAccount()
        {
            ViewBag.Message = "Please enter the following information to create your account";
            return View("CreateAccount");
        }

        [HttpPost]
        [AsyncTimeout(Globals.patienceLevel_ms)]
        public ActionResult CreateAccount(string textUsername, string textPassword, string textAddress, string textPhoneNumber, string textEmail, bool accountType)
        {
            //TODO: check entered values for validity before sending

            CreateAccount msg = new CreateAccount
            {
                username = textUsername,
                password = textPassword,
                address = textAddress,
                phonenumber = textPhoneNumber,
                email = textEmail,
                type = accountType ? AccountType.business : AccountType.user
            };

            string response = ServiceBusCommunicationManager.sendNewAccountInfo(msg);


            //TODO: React based on the response

            return RedirectToAction("Index", "Home");
        }
    }
}