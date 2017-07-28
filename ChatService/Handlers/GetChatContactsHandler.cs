using ChatService.Database;

using Messages.Commands;

using NServiceBus;
using NServiceBus.Logging;

using System.Threading.Tasks;

namespace ChatService.Handlers
{
    public class GetChatContactsHandler : IHandleMessages<GetChatContacts>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        static ILog log = LogManager.GetLogger<GetChatContactsHandler>();

        /// <summary>
        /// Searches the chat database for the names of all other users the given user has made chat contact with
        /// </summary>
        /// <param name="message">The message containing the usersname</param>
        /// <param name="context">the endpoint that received this</param>
        /// <returns>The same message with the contactnames property filled</returns>
        public Task Handle(GetChatContacts message, IMessageHandlerContext context)
        {
            //return Task.CompletedTask;
            message.contactNames = ChatServiceDatabase.getInstance().getAllChatContactsForUser(message.usersname);

            return context.Reply(message);
        }
    }
}
