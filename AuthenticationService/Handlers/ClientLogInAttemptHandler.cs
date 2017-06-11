using Messages.Events;

using NServiceBus;
using NServiceBus.Logging;

using System;
using System.Threading.Tasks;

namespace AuthenticationService.Handlers
{
    /// <summary>
    /// This class is responsible for handling login requests from the web client
    /// </summary>
    public class ClientLogInAttemptHandler : IHandleMessages<ClientLogInAttempt>
    {
        /// <summary>
        /// A log that will output messages to the command line as well as to a log file.
        /// </summary>
        static ILog log = LogManager.GetLogger<ClientLogInAttemptHandler>();

        /// <summary>
        /// This is the function that is called when a client attempts to log in using some credentials
        /// </summary>
        /// <param name="message">The message itself, containing the relevant information</param>
        /// <param name="context">Contains information about where the message came from</param>
        /// <returns>A Task that will give the sending client a response</returns>
        public Task Handle(ClientLogInAttempt message, IMessageHandlerContext context)
        {
            log.Info("Recieved a request for authentication with credentials:\n" +
                        "\tUsername:<" + message.username + ">\n" +
                        "\tPassword <" + message.password + ">");

            return context.Publish(message);
        }
    }
}
