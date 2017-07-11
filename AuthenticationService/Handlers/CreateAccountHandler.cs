using AuthenticationService.Database;

using Messages.Commands;
using Messages.Events;

using NServiceBus;
using NServiceBus.Logging;

using System;
using System.Threading.Tasks;

namespace AuthenticationService.Handlers
{
    public class CreateAccountHandler : IHandleMessages<CreateAccount>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        static ILog log = LogManager.GetLogger<CreateAccountHandler>();

        /// <summary>
        /// Creates a new account in authentication database and returns an AccountCreated event to be published to all subscribers
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Handle(CreateAccount msg, IMessageHandlerContext context)
        {
            AuthenticationDatabase.getInstance().insertNewUserAccount(msg);

            AccountCreated Event = new AccountCreated(msg);

            return context.Publish(Event);
        }
    }
}
