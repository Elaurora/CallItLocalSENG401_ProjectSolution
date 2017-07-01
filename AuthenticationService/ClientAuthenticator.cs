using Messages.Events;

using NServiceBus;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AuthenticationService
{
    /// <summary>
    /// This portion of the class contains the member variables,
    /// as well as the getters and setters
    /// </summary>
    partial class ClientAuthenticator
    {
        private const int timeout_ms = 50;

        /// <summary>
        /// The connection between the client and the server
        /// </summary>
        private Socket connection;

        /// <summary>
        /// The endpoint, used to communicate with other services in the bus.
        /// All clients use the same endpoint
        /// </summary>
        private IEndpointInstance authenticationEndpoint;

        /// <summary>
        /// The username given by the client
        /// </summary>
        private string username = String.Empty;

        /// <summary>
        /// the password given by the client
        /// </summary>
        private string password = String.Empty;

        /// <summary>
        /// Indicates whether or not the username and password given by the client are valid
        /// </summary>
        private bool authenticated = false;

        public bool isAuthenticated()
        {
            return authenticated;
        }
    }

    /// <summary>
    /// This portion of the class contains function definitions
    /// </summary>
    partial class ClientAuthenticator
    {

        public ClientAuthenticator(Socket connection, IEndpointInstance authenticationEndpoint)
        {
            this.connection = connection;
            this.authenticationEndpoint = authenticationEndpoint;

            connection.ReceiveTimeout = timeout_ms;
        }

        /// <summary>
        /// Waits for the client to send a username and a password though the socket,
        /// and check the validity of the information once it is recieved.
        /// </summary>
        public void readLoginInfo()
        {
            username = readUntilEOF();
            password = readUntilEOF();
            attemptToAuthenticate();
        }

        /// <summary>
        /// Continuously reads one byte at a time from the client until the "<EOF>" string of characters is found
        /// </summary>
        /// <returns>The string representation of bytes read from the client socket</returns>
        public string readUntilEOF()
        {
            byte[] readByte = new byte[1];
            string returned = String.Empty;

            while (returned.Contains("<EOF>") == false)
            {
                try
                {
                    connection.Receive(readByte, 1, 0);
                    returned += (char)readByte[0];
                }
                catch(SocketException e)// This is thrown when the timeout occurs. The timeout is set in the constructor
                {
                    Thread.Yield();// Yield this threads remaining timeslice to another process, this process does not appear to need it
                }
            }

            return returned.Substring(0, returned.IndexOf("<EOF>"));
        }

        /// <summary>
        /// Checks to see if the username and password given by the client are valid
        /// NOT IMPLEMENTED. Currently, this function will just authenticate the user regardless of
        /// the information that was entered
        /// </summary>
        private void attemptToAuthenticate()
        {
            //TODO: Implement logic to authenticate user assuming the username and password have been read

            authenticated = true;
            reportLogInAttempt();
        }

        /// <summary>
        /// Publishes a ClientLogInAttempted event through the endpoint to all subscribers
        /// </summary>
        private void reportLogInAttempt()
        {
            ClientLogInAttempted attempt = new ClientLogInAttempted
            {
                username = this.username,
                password = this.password,
                clientAccepted = this.authenticated,
                requestSource = ((IPEndPoint)connection.RemoteEndPoint).Serialize()
            };

            //Publish the log in attempt event for any other EP that wish to know about it.
            //If an endpoint wishes to be notified about this event, it should subscribe to the event in its configuration
            Console.Write("Log in attempted with credentials:" + "\n" +
                "Username:" + username + "\n" +
                "Password:" + password + "\n");
            authenticationEndpoint.Publish(attempt);
        }

       
    }

    
}
