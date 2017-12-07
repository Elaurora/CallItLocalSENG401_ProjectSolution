using CompanyDirectoryService.Database;

using NServiceBus;
using NServiceBus.Logging;

using Messages.NServiceBus.Events;
using Messages.DataTypes;

using System;
using System.Threading.Tasks;

namespace CompanyDirectoryService.Handlers
{
    /// <summary>
    /// This class is used by the Company Directory Service endpoint when the authentication service publishes an AccountCreated event.
    /// </summary>
    public class AccountCreatedHandler : IHandleMessages<AccountCreated>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        private static ILog log = LogManager.GetLogger<AccountCreatedHandler>();

        /// <summary>
        /// This handler will add the newly created account to the list of companies upon the creation of a business account
        /// This function will do nothing if the new account is a regular user account
        /// </summary>
        /// <param name="message">The event object that was published</param>
        /// <param name="context">Contains information relevent to the current event being handled.</param>
        /// <returns>A completed task, which basically means it returns nothing</returns>
        public Task Handle(AccountCreated message, IMessageHandlerContext context)
        {
            //return Task.CompletedTask;
            if(message.type == AccountType.business)
            {
                if(CompanyDirectoryDB.getInstance().insertNewCompany(message) == false)
                {
                    throw new Exception("Failed to enter company into database;");
                }
            }
            return Task.CompletedTask;
        }
    }
}
