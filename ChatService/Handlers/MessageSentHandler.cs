using ChatService.Database;

using Messages.Events;

using NServiceBus;
using NServiceBus.Logging;

using System.Threading.Tasks;

namespace ChatService.Handlers
{
    /// <summary>
    /// This class is used by the Chat Service endpoint when the Authentication services publishing endpoint publishes a MessageSent event
    /// </summary>
    public class MessageSentHandler : IHandleMessages<MessageSent>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        static ILog log = LogManager.GetLogger<MessageSentHandler>();

        /// <summary>
        /// Saves the message to the Chat Service database
        /// </summary>
        /// <param name="message">The event object that was published</param>
        /// <param name="context">Contains information relevent to the current event being handled.</param>
        /// <returns>A completed task, which basically means it returns nothing</returns>
        public Task Handle(MessageSent message, IMessageHandlerContext context)
        {
            ChatServiceDatabase.getInstance().saveMessage(message.msg);
            return Task.CompletedTask;
        }
    }
}
