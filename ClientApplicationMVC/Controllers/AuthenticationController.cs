using System.Threading.Tasks;
using System.Web.Mvc;

using Messages;
using Messages.Commands; 

using NServiceBus;

namespace ClientApplicationMVC.Controllers
{
    public class AuthenticationController : AsyncController
    {
        IEndpointInstance endpoint;

        public AuthenticationController(IEndpointInstance endpoint)
        {
            this.endpoint = endpoint;
        }

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
        public async Task<ActionResult> IndexAsync(string textUsername, string textPassword)
        {
            //Create the message object that will be sent to the Authentication service
            var loginInfo = new AuthenticateMe
            {
                username = textUsername,
                password = textPassword
            };

            var sendOptions = new SendOptions();
            sendOptions.SetDestination("Authentication");//Set the destination of the message to be the endpoint with the given name. Its address is defined in TODO: Finish this comment

            //Send the Command to the AuthenticationService and await its response before continueing
            var response = await endpoint.Request<AuthenticationResult>(loginInfo, sendOptions).ConfigureAwait(false);

            if(response.success == false)
            {
                //TODO: Implement the logic for how the website should respond if it fails to authenticate.
                // This will include returning a different view to be displayed
                //throw new NotImplementedException("Have not implemented logic for failed authentication");
            }

            ViewBag.Title = "AuthenticationSuccess";
            ViewBag.Result = response.success.ToString();
            return View("AuthenticationSuccess");
        }
    }
}