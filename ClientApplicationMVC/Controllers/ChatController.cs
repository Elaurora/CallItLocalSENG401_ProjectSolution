using ClientApplicationMVC.Models;

using Messages.DataTypes.Database.Chat;

using System.Web.Mvc;

namespace ClientApplicationMVC.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        public ActionResult Index()
        {
            if(Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }

            //TODO: Get all of the users current chat instances as well as all of the messages of the first instance from the chat service
            string[] chatInstances = ServiceBusCommunicationManager.getAllChatContacts();

            ChatHistory firstDisplayedChatHistory = ServiceBusCommunicationManager.getChatHistory(chatInstances[0]);

            ViewBag.ChatInstances = chatInstances;
            ViewBag.DisplayedChatHistory = firstDisplayedChatHistory;

            return View();
        }

        [HttpPost]
        public ActionResult SendMessage(string receiver, int unix_timestamp, string messageContents)
        {
            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ChatMessage message = new ChatMessage
            {
                sender = Globals.getUser(),
                receiver = receiver,
                unix_timestamp = unix_timestamp,
                messageContents = messageContents
            };

            ServiceBusCommunicationManager.sendChatMessage(message);

            //TODO: Attempt to send the message to the receiver if they currently have an open session

            return null;
        }
    }
}