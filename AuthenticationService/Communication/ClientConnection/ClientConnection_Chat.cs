using Messages.NServiceBus.Commands;
using Messages.DataTypes.Database.Chat;
using Messages.NServiceBus.Events;
using Messages.ServiceBusRequest;
using Messages.ServiceBusRequest.Chat;
using Messages.ServiceBusRequest.Chat.Requests;
using Messages.ServiceBusRequest.Chat.Responses;

using NServiceBus;

using System.Collections.Generic;

namespace AuthenticationService.Communication
{
    /// <summary>
    /// This portion of the class contains the methods used specifically for accessing the chat service
    /// </summary>
    partial class ClientConnection
    {
        /// <summary>
        /// This function will execute the task requested of the chat service and return a response.
        /// </summary>
        /// <param name="requestParameters">The information regarding which task is requested and any additional information needed</param>
        /// <returns>A response from the chat service</returns>
        private ServiceBusResponse chatRequest(ChatServiceRequest request)
        {
            switch (request.requestType)
            {
                case (ChatRequest.sendMessage):
                    return messageSent((SendMessageRequest)request);
                case (ChatRequest.getChatContacts):
                    return getAllChatContactsForUser((GetChatContactsRequest)request);
                case (ChatRequest.getChatHistory):
                    return getChatHistory((GetChatHistoryRequest)request);
                default:
                    return new ServiceBusResponse(false, "Error: Invalid request. Did not specify a valid request from the chat service.");
            }
        }
        
        /// <summary>
        /// Publishes a MessageSent event through the endpoint.
        /// </summary>
        /// <param name="messageInfo">Information about the message, ready to be parsed by the ChatMessage in the proper format</param>
        /// <returns>An empty string upon success</returns>
        private ServiceBusResponse messageSent(SendMessageRequest request)
        {
            if(authenticated == false)
            {
                return new ServiceBusResponse(false, "You must be logged in to use the chat service");
            }

            ChatMessage msg = request.message;
            eventPublishingEndpoint.Publish(new MessageSent { msg = msg });
            return new ServiceBusResponse(true, "");
        }

        /// <summary>
        /// Asks the ChatService endpoint to get a list of usernames the given user has contacted via chat in the past
        /// </summary>
        /// <param name="usersname">The name of the user</param>
        /// <returns>A single string containing the usernames of users the given user has contacted via chat. A " " response indicates no other users were found</returns>
        private GetChatContactsResponse getAllChatContactsForUser(GetChatContactsRequest request)
        {
            if (authenticated == false)
            {
                return new GetChatContactsResponse(false, "You must be logged in to use the chat service", null);
            }

            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("Chat");
            
            return requestingEndpoint.Request<GetChatContactsResponse>(request, sendOptions).
                ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sends a request to the chat service for the chat history between the 2 specified users.
        /// </summary>
        /// <param name="info">Should contain the two users whos chat history is being requested. in the form "userone=whatev1&usertwo=whatev2"</param>
        /// <returns>A string representation of the chat history between the 2 given users.</returns>
        private GetChatHistoryResponse getChatHistory(GetChatHistoryRequest request)
        {
            if (authenticated == false)
            {
                return new GetChatHistoryResponse(false, "You must be logged in to use the chat service", null);
            }

            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("Chat");

            return requestingEndpoint.Request<GetChatHistoryResponse>(request, sendOptions)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
