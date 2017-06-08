using Messages.Commands;
using Messages;

using NServiceBus;
using NServiceBus.Logging;

using System;
using System.Threading.Tasks;

namespace AuthenticationService.Handlers
{
    /// <summary>
    /// This class is responsible for handling login requests from the web client
    /// </summary>
    public class AuthenticateMeHandler : IHandleMessages<AuthenticateMe>
    {
        /// <summary>
        /// A log that will output messages to the command line as well as to a log file.
        /// </summary>
        static ILog log = LogManager.GetLogger<AuthenticateMeHandler>();

        /// <summary>
        /// This is the function that is called when a client attempts to log in using some credentials
        /// </summary>
        /// <param name="message">The message itself, containing the relevant information</param>
        /// <param name="context">Contains information about where the message came from</param>
        /// <returns>A Task that will give the sending client a response</returns>
        public Task Handle(AuthenticateMe message, IMessageHandlerContext context)
        {
            log.Info("Recieved a request for authentication with credentials:\n" +
                        "\tUsername:<" + message.username + ">\n" +
                        "\tPassword <" + message.password + ">");

            var authenticated = new AuthenticationResult
            {
                success = true
            };

            //TODO: Implement authentication logic here

            if (authenticated.success == false)
            {
                //TODO: Implement the logic deciding what to do & how to reply in the case of a failed authentication here
                throw new NotImplementedException();
            }

            //Give a Success response back to the sender of the message
            return context.Reply(authenticated);

        }
    }
}
