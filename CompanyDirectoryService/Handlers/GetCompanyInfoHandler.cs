using CompanyDirectoryService.Database;

using Messages.Commands;
using Messages.DataTypes.Database.CompanyDirectory;

using NServiceBus;
using NServiceBus.Logging;

using System.Threading.Tasks;

namespace CompanyDirectoryService.Handlers
{
    /// <summary>
    /// This class is used by the Company Directory Service endpoint when a client requests information about a specific company
    /// </summary>
    public class GetCompanyInfoHandler : IHandleMessages<GetCompanyInfo>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        private static ILog log = LogManager.GetLogger<GetCompanyInfoHandler>();

        /// <summary>
        /// Searches the CompanyDirectory database for a company matching the one in the comand object and returns its information if it exists
        /// </summary>
        /// <param name="message">The command object that was sent</param>
        /// <param name="context">Contains information relevent to the current command being handled.</param>
        /// <returns>A CompanyInstance response containing information about the requested company, or null if the company was not found</returns>
        public Task Handle(GetCompanyInfo message, IMessageHandlerContext context)
        {
            CompanyInstance response = CompanyDirectoryDB.getInstance().getCompanyInfo(message.companyName);

            return context.Reply(response);
        }
    }
}
