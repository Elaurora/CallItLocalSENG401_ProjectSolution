using ChatService.Database;

using Messages.Events;

using NServiceBus;
using NServiceBus.Logging;

using System.Threading.Tasks;

namespace ChatService.Handlers
{
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
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Handle(MessageSent message, IMessageHandlerContext context)
        {
            ChatServiceDatabase.getInstance().saveMessage(message.msg);
            return Task.CompletedTask;
        }
    }
}
