using ClientApplicationMVC.Models;

using Messages.DataTypes.Database.Chat;

using System.Web.Mvc;

namespace ClientApplicationMVC.Controllers
{
    [Route("Chat")]
    public class ChatController : Controller
    {
        
        [HttpGet]
        public ActionResult Index()
        {
            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }

            //TODO: Get all of the users current chat instances as well as all of the messages of the first instance from the chat service
            string[] chatInstances = ServiceBusCommunicationManager.getAllChatContacts();
            ChatHistory firstDisplayedChatHistory = chatInstances.Length > 0 ? ServiceBusCommunicationManager.getChatHistory(chatInstances[0]) : new ChatHistory();

            ViewBag.ChatInstances = chatInstances;
            ViewBag.DisplayedChatHistory = firstDisplayedChatHistory;

            return View();
        }

        [HttpPost]
        public ActionResult SendMessage(string receiver = "", int timestamp = -1, string message = "")
        {
            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }

            if("".Equals(receiver) || "".Equals(message) || timestamp == -1)
            {
                throw new System.Exception("Did not supply all required arguments.");
            }

            ChatMessage Message = new ChatMessage
            {
                sender = Globals.getUser(),
                receiver = receiver,
                unix_timestamp = timestamp,
                messageContents = message
            };

            sendMessageToBus(Message);
            return null;
        }

        /// <summary>
        /// Send the given chat message to the bus to be added to the database
        /// </summary>
        /// <param name="msg">The message to be sent to the bus</param>
        private void sendMessageToBus(ChatMessage msg)
        {
            ServiceBusCommunicationManager.sendChatMessage(msg);
            //TODO: Attempt to send the message to the receiver if they currently have an open session
            //BUT HOOOOOWWWWW

        }
    }
}
