using Messages.Events;

using NServiceBus;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AuthenticationService
{
    /// <summary>
    /// This portion of the class contains the member variables
    /// </summary>
    partial class ClientConnection
    {
        /// <summary>
        /// The endpoint, used to communicate with other services in the bus.
        /// All clients use the same endpoint
        /// </summary>
        IEndpointInstance authenticationEndpoint;

        /// <summary>
        /// The connection between the client and the server.
        /// </summary>
        Socket connection;

        /// <summary>
        /// This class will be used to authenticate the client
        /// </summary>
        ClientAuthenticator authenticator;
    }

    /// <summary>
    /// This portion of the class contains the function definitions
    /// 
    /// This class is used to communicate with clients that wish to use the services the bus has
    /// The client will continue to attempt to read a username and password until a valid pair has
    /// been given
    /// </summary>
    partial class ClientConnection
    {
        public ClientConnection(Socket connection, IEndpointInstance authenticationEndpoint)
        {
            this.connection = connection;
            this.authenticationEndpoint = authenticationEndpoint;
            authenticator = new ClientAuthenticator(connection, authenticationEndpoint);
        }

        /// <summary>
        /// Waits for the client to be authenticated, then listens for message requests/commands until the socket is closed.
        /// </summary>
        public async void listenToClient()
        {
            waitUntilAuthenticated();
            while (true)
            {
                //TODO: Implement logic to deal with other messages that the web client may send

                //Read the name of the service the client wishes to access
                string serviceRequested = authenticator.readUntilEOF();

                
            }
        }

        /// <summary>
        /// Wait for username and password to be sent, check the validity of the information sent,
        /// rinse repeat until authenticated. Sends a message to the client indicating whether or not 
        /// it was authenticated successfully
        /// </summary>
        private void waitUntilAuthenticated()
        {
            while (true)
            {
                authenticator.readLoginInfo();
                if(authenticator.isAuthenticated() == true)
                {
                    connection.Send(Encoding.ASCII.GetBytes("You have been logged in successfully<EOF>"));
                    break;
                }
                connection.Send(Encoding.ASCII.GetBytes("Incorrect username or password<EOF>"));
            }
        }
    }
}
