using ChatService.Database;

using Messages.Commands;

using NServiceBus;
using NServiceBus.Logging;

using System.Threading.Tasks;

namespace ChatService.Handlers
{
    /// <summary>
    /// This class is used by the Chat Service endpoint when a client requests their chat history with a specific user
    /// </summary>
    public class GetChatHistoryHandler : IHandleMessages<GetChatHistory>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        static ILog log = LogManager.GetLogger<GetChatHistoryHandler>();

        /// <summary>
        /// Asks the database to retrieve all chat messages passed between the 2 users specified in the command object.
        /// </summary>
        /// <param name="message">The command containg the message information</param>
        /// <param name="context">The receiving endpoint</param>
        /// <returns>The command object with the history property filled</returns>
        public Task Handle(GetChatHistory message, IMessageHandlerContext context)
        {
            message.history = ChatServiceDatabase.getInstance().getChatHistory(message.userone, message.usertwo);

            return context.Reply(message);
        }
    }
}