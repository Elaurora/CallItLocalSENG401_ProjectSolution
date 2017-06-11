using Messages.Events;

using NServiceBus;

using System.Net.Sockets;
using System.Text;

namespace AuthenticationService
{
    partial class ClientConnection
    {
        IEndpointInstance clientEndpoint;

        Socket connection;

        ClientAuthenticator authenticator;
    }

    partial class ClientConnection
    {
        public ClientConnection(Socket connection, IEndpointInstance clientEndpoint)
        {
            this.connection = connection;
            this.clientEndpoint = clientEndpoint;
            authenticator = new ClientAuthenticator(connection, clientEndpoint);
        }

        public void listenToClient()
        {
            waitUntilAuthenticated();
            
            while (true)
            {
               //TODO: Implement logic to deal with other messages that the web client may send
            }
        }

        private void waitUntilAuthenticated()
        {
            while (true)
            {
                authenticator.readLoginInfo();
                authenticator.attemptToAuthenticate();
                if(authenticator.isAuthenticated() == true)
                {
                    break;
                }
                connection.Send(Encoding.ASCII.GetBytes("Incorrect username or password<EOF>"));
            }
        }

        
    }
}
