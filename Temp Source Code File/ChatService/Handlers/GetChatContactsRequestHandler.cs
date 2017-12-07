using ChatService.Database;

using Messages.ServiceBusRequest.Chat.Responses;
using Messages.ServiceBusRequest.Chat.Requests;

using NServiceBus;
using NServiceBus.Logging;

using System.Threading.Tasks;

namespace ChatService.Handlers
{
    /// <summary>
    /// This class is used by the Chat Service endpoint when a client requests a list of its ChatContacts
    /// </summary>
    public class GetChatContactsRequestHandler : IHandleMessages<GetChatContactsRequest>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        static ILog log = LogManager.GetLogger<GetChatContactsRequestHandler>();

        /// <summary>
        /// Searches the chat database for the names of all other users the given user has made chat contact with
        /// </summary>
        /// <param name="message">The command object that was sent</param>
        /// <param name="context">Contains information relevent to the current command being handled.</param>
        /// <returns>The command object with the contactnames property filled</returns>
        public Task Handle(GetChatContactsRequest message, IMessageHandlerContext context)
        {
            GetChatContactsResponse dbResponse = ChatServiceDatabase.getInstance().getAllChatContactsForUser(message);

            return context.Reply(dbResponse);
        }
    }
}
