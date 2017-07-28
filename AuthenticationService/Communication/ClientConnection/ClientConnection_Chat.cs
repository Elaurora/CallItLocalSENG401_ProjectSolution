using Messages.Commands;
using Messages.DataTypes.Database.Chat;
using Messages.Events;

using NServiceBus;

using System.Collections.Generic;

namespace AuthenticationService.Communication
{
    partial class ClientConnection
    {
        /// <summary>
        /// This function will execute the task requested of the chat service and return a response.
        /// </summary>
        /// <param name="requestParameters">The information regarding which task is requested and any additional information needed</param>
        /// <returns>A response from the chat service</returns>
        private string chatRequest(List<string> requestParameters)
        {
            if(authenticated == false)
            {
                return ("You must be logged in to use the chat service.");
            }

            string taskRequested = requestParameters[0];
            requestParameters.RemoveAt(0);

            switch (taskRequested)
            {
                case ("sendmessage"):
                    return messageSent(requestParameters[0]);
                case ("getchatcontacts"):
                    return getAllChatContactsForUser(requestParameters[0]);
                case ("getchathistory"):
                    return getChatHistory(requestParameters[0]);
                default:
                    return ("Error: Invalid request. Did not specify a valid request from the chat service.");
            }
        }


        /// <summary>
        /// Publishes a MessageSent event through the endpoint.
        /// </summary>
        /// <param name="messageInfo">Information about the message, ready to be parsed by the ChatMessage in the proper format</param>
        /// <returns>An empty string upon success</returns>
        private string messageSent(string messageInfo)
        {
            ChatMessage msg = new ChatMessage(messageInfo);
            eventPublishingEndpoint.Publish(new MessageSent { msg = msg });
            return "";
        }

        /// <summary>
        /// Asks the ChatService endpoint to get a list of usernames the given user has contacted via chat in the past
        /// </summary>
        /// <param name="usersname">The name of the user</param>
        /// <returns>A single string containing the usernames of users the given user has contacted via chat. A " " response indicates no other users were found</returns>
        private string getAllChatContactsForUser(string givenuser)
        {
            //TODO: Learn about how databases work on a lower level

            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("Chat");

            GetChatContacts request = new GetChatContacts
            {
                usersname = givenuser
            };

            request = requestingEndpoint.Request<GetChatContacts>(request, sendOptions).
                ConfigureAwait(false).GetAwaiter().GetResult();

            if(request.contactNames == null || request.contactNames.Count < 1)
            {
                return " ";// Return a space to indicate no users found
            }
            string returned = "";

            for(int i = 0; i != request.contactNames.Count; i++)
            {
                if(i != 0)
                {
                    returned += "&";
                }
                returned += request.contactNames[i];
            }
            return returned;
        }

        /// <summary>
        /// Sends a request to the chat service for the chat history between the 2 specified users.
        /// </summary>
        /// <param name="info">Should contain the two users whos chat history is being requested. in the form "userone=whatev1&usertwo=whatev2"</param>
        /// <returns>A string representation of the chat history between the 2 given users.</returns>
        private string getChatHistory(string info)
        {
            string[] parts = info.Split('&');

            GetChatHistory request = new GetChatHistory();

            foreach(string part in parts)
            {
                string[] pieces = part.Split('=');
                switch (pieces[0])
                {
                    case ("userone"):
                        request.userone = pieces[1];
                        break;
                    case ("usertwo"):
                        request.usertwo = pieces[1];
                        break;
                    default:
                        return ("Error: Invalid format for get Chat History parameters");
                }
            }

            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("Chat");

            request = requestingEndpoint.Request<GetChatHistory>(request, sendOptions)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            return request.history.toString();
        }
    }
}
