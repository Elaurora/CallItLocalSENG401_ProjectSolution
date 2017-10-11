using ClientApplicationMVC.Models;

using Messages.DataTypes.Database.Chat;

using System.Web.Mvc;

namespace ClientApplicationMVC.Controllers
{
    /// <summary>
    /// This class contains the functions responsible for handling requests routed to *Hostname*/Chat/*
    /// </summary>
    [Route("Chat")]
    public class ChatController : Controller
    {
        /// <summary>
        /// This function is called when the client navigates to *hostname*/Chat
        /// </summary>
        /// <returns>A view to be sent to the client</returns>
        [HttpGet]
        public ActionResult Index()
        {
            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ServiceBusConnection connection = ServiceBusCommunicationManager.getConnectionObject(Globals.getUser());
            if (connection == null)
            {
                return RedirectToAction("Index", "Authentication");
            }


            string[] chatInstances = connection.getAllChatContacts();
            ChatHistory firstDisplayedChatHistory = chatInstances.Length > 0 ? connection.getChatHistory(chatInstances[0]) : new ChatHistory();

            ViewBag.ChatInstances = chatInstances;
            ViewBag.DisplayedChatHistory = firstDisplayedChatHistory;

            return View();
        }

        /// <summary>
        /// This function is called when an ajax call is made indicating that a message has been sent
        /// </summary>
        /// <param name="receiver">The username of the user the message is being sent to</param>
        /// <param name="timestamp">A unix timestamp indicating the time the message was sent</param>
        /// <param name="message">The content of the message</param>
        /// <returns>Nothing</returns>
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

            ServiceBusConnection connection = ServiceBusCommunicationManager.getConnectionObject(Globals.getUser());
            if (connection == null)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ChatMessage Message = new ChatMessage
            {
                sender = Globals.getUser(),
                receiver = receiver,
                unix_timestamp = timestamp,
                messageContents = message
            };

            connection.sendChatMessage(Message);
            return null;
        }

        /// <summary>
        /// This function is called when an ajax call is made requesting the chat history between the current user and another specified user
        /// </summary>
        /// <param name="otherUser">The name of the user to get the chat history with the current user</param>
        /// <returns>The chat history between the two users</returns>
        [HttpGet]
        public ActionResult Conversation(string otherUser = "")
        {
            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }
            if ("".Equals(otherUser))
            {
                throw new System.Exception("Did not supply all required arguments.");
            }

            ServiceBusConnection connection = ServiceBusCommunicationManager.getConnectionObject(Globals.getUser());
            if (connection == null)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ChatHistory userHistory = connection.getChatHistory(otherUser);

            string newConvoHtml = "";

            foreach(ChatMessage msg in userHistory.messages)
            {
                if (msg.sender.Equals(Globals.getUser()))
                {
                    newConvoHtml +=
                        "<p class=\"message\">" +
                            "<span class=\"username\">You: </span>" +
                            msg.messageContents +
                        "</p>";
                }
                else
                {
                    newConvoHtml +=
                        "<p class=\"message\">" +
                            "<span class=\"username\" style=\"color:aqua;\">" + msg.sender + ": </span>" +
                            msg.messageContents +
                        "</p>";
                }
            }

            return Content(newConvoHtml);
        }
    }
}
