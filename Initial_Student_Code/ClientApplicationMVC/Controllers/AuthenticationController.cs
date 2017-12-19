using ClientApplicationMVC.Models;

using Messages.NServiceBus.Commands;
using Messages.DataTypes;
using Messages.ServiceBusRequest;
using Messages.ServiceBusRequest.Authentication.Requests;

using System.Web.Mvc;

namespace ClientApplicationMVC.Controllers
{
    /// <summary>
    /// This class contains the functions responsible for handling requests routed to *Hostname*/Authentication/*
    /// </summary>
    public class AuthenticationController : Controller
    {
        /// <summary>
        /// The default method for this controller
        /// </summary>
        /// <returns>The login page</returns>
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
            LogInRequest request = new LogInRequest(textUsername, textPassword);

            ServiceBusResponse response = ConnectionManager.sendLogIn(request);
            
            if (response.result == true)
            {
                ViewBag.Title = "Authentication Success";
                Globals.setUser(textUsername);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Title = "Authentication Failure";
            ViewBag.Message = response.response;

            return View("Index");
        }

        /// <summary>
        /// This function is called when the client navigates to *hostname*/Authentication/CreateAccount
        /// </summary>
        /// <returns>A view to be sent to the client containg the html page to create an account</returns>
        [HttpGet]
        public ActionResult CreateAccount()
        {
            ViewBag.Message = "Please enter the following information to create your account";
            ViewBag.AccountCreationResult = "NotAttempted";
            return View("CreateAccount");
        }

        /// <summary>
        /// This controller is used when an httpPost request is made to /Authentication/CreateAccount
        /// If the account creation is successful the user will be logged in.
        /// </summary>
        /// <param name="textUsername">The username for the new account</param>
        /// <param name="textPassword">The password for the new account</param>
        /// <param name="textAddress">The address for the new account</param>
        /// <param name="textPhoneNumber">The phone number for the new account</param>
        /// <param name="textEmail">The email for the new account</param>
        /// <param name="accountType">The account type</param>
        /// <returns>A redirect to a different page, the current page if unsuccessful and the home page if successsful</returns>
        [HttpPost]
        [AsyncTimeout(Globals.patienceLevel_ms)]
        public ActionResult CreateAccount(string textUsername, string textPassword, string textAddress, string textPhoneNumber, string textEmail, bool accountType)
        {
            CreateAccount msg = new CreateAccount
            {
                username = textUsername,
                password = textPassword,
                address = textAddress,
                phonenumber = textPhoneNumber,
                email = textEmail,
                type = accountType ? AccountType.business : AccountType.user
            };

            CreateAccountRequest request = new CreateAccountRequest(msg);

            ServiceBusResponse response = ConnectionManager.sendNewAccountInfo(request);

            if (response.result == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.AccountCreationResult = response.response;

            return View("CreateAccount");
        }
    }
}