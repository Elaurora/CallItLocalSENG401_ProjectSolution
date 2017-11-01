using CompanyDirectoryService.Database;

using Messages.ServiceBusRequest.CompanyDirectory.Responses;
using Messages.ServiceBusRequest.CompanyDirectory.Requests;

using NServiceBus;
using NServiceBus.Logging;

using System.Threading.Tasks;

namespace CompanyDirectoryService.Handlers
{
    /// <summary>
    /// This class is used by the Company Directory Service endpoint when a client requests a list of companies matching the given description
    /// </summary>
    public class CompanySearchRequestHandler : IHandleMessages<CompanySearchRequest>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        private static ILog log = LogManager.GetLogger<CompanySearchRequestHandler>();

        /// <summary>
        /// Searches the Company Directory Service database for any company's that match the description given in the command object
        /// </summary>
        /// <param name="message">The command object that was sent</param>
        /// <param name="context">Contains information relevent to the current command being handled.</param>
        /// <returns>An object containing a list of companies matching the given description</returns>
        public Task Handle(CompanySearchRequest message, IMessageHandlerContext context)
        {
            CompanySearchResponse dbResponse = CompanyDirectoryDB.getInstance().searchByName(message);

            return context.Reply(dbResponse);
        }
    }
}
