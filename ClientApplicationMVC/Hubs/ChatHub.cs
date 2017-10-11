using ClientApplicationMVC.Models;

using Messages.DataTypes.Collections;
using Messages.DataTypes.Database.Chat;

using Microsoft.AspNet.SignalR;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientApplicationMVC.Hubs
{
    /// <summary>
    /// This class contains the methods that can be called by the client.
    /// This is also the class whos functions can call functions on the client
    /// </summary>
    public partial class ChatHub : Hub
    {
        /// <summary>
        /// This method is called by the client when it first conencts to the hub. 
        /// The client indicates its username so that this class may access the service bus.
        /// </summary>
        /// <param name="username">Tye username of the client that just connected.</param>
        public void hello(string username)
        {
            string connectionId = Context.ConnectionId;

            connectedUsers.Add(username, connectionId);

            string thing = this.Context.User.Identity.Name;
        }

        /// <summary>
        /// This function is called by the client when a user sends a message.
        /// </summary>
        /// <param name="message">The message being sent</param>
        /// <param name="receiver">The username of the receiver of the message</param>
        /// <param name="timsestamp">The time at which the message was sent</param>
        public void sendMessageTo(string message, string receiver, int timsestamp)
        {
            string receiverConnectionID = connectedUsers.getConnectionID(receiver);
            string user = connectedUsers.getUsername(Context.ConnectionId);

            ChatMessage Message = new ChatMessage
            {
                sender = user,
                receiver = receiver,
                unix_timestamp = timsestamp,
                messageContents = message
            };

            //Send the chat message to the service bus to be saved in the database
            ServiceBusCommunicationManager.sendChatMessage(Message);
             
            if (receiverConnectionID != null)
            {
                Clients.Client(receiverConnectionID).receiveMessage(message, user);
                
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId = Context.ConnectionId;

            connectedUsers.RemoveByConnectionID(connectionId);

            return base.OnDisconnected(stopCalled);
        }
        
    }

    /// <summary>
    /// This portion of the class contains the member variables
    /// </summary>
    public partial class ChatHub : Hub
    {
        private static readonly ConnectionDictionary connectedUsers =
            new ConnectionDictionary();
    }
}