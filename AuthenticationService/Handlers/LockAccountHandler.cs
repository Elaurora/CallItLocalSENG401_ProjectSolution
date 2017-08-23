using Messages.Commands;
using Messages.Events;

using NServiceBus;
using NServiceBus.Logging;

using System;
using System.Threading.Tasks;

namespace AuthenticationService.Handlers
{
    /// <summary>
    /// This class is used by the authentication service event publishing endpoint when the LoginLogger service sends a LockAccount command
    /// </summary>
    public class LockAccountHandler : IHandleMessages<LockAccount>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        static ILog log = LogManager.GetLogger<LockAccountHandler>();

        /// <summary>
        /// This handler will lock the account using ????
        /// </summary>
        /// <param name="message">The command object that was sent</param>
        /// <param name="context">Contains information relevent to the current command being handled.</param>
        /// <returns>A task to publish an AccountLocked event</returns>
        public Task Handle(LockAccount message, IMessageHandlerContext context)
        {
            log.Info("Recieved a command to lock an account.", new NotImplementedException());

            //TODO MOSHI low importance: Implement logic to lock an account.

            var accountLocked = new AccountLocked
            {
                //TODO MOSHI low importance: Populate this with relevant info, once AccountLocked has been implemented.
            };

            return context.Publish(accountLocked);
        }
    }
}
