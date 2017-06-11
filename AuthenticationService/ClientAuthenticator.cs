using Messages.Events;

using NServiceBus;

using System;
using System.Net;
using System.Net.Sockets;

namespace AuthenticationService
{
    partial class ClientAuthenticator
    {

        /// <summary>
        /// 
        /// </summary>
        private Socket connection;

        /// <summary>
        /// 
        /// </summary>
        private IEndpointInstance authenticationEndpoint;

        /// <summary>
        /// 
        /// </summary>
        private string username = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        private string password = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        private bool authenticated = false;

        public bool isAuthenticated()
        {
            return authenticated;
        }


    }

    partial class ClientAuthenticator
    {

        public ClientAuthenticator(Socket connection, IEndpointInstance authenticationEndpoint)
        {
            this.connection = connection;
            this.authenticationEndpoint = authenticationEndpoint;
        }

        public void readLoginInfo()
        {
            username = readUntilEOF();
            password = readUntilEOF();
        }

        private string readUntilEOF()
        {
            byte[] readByte = new byte[1];
            string returned = String.Empty;

            while (returned.Contains("<EOF>") == false)
            {
                connection.Receive(readByte, 1, 0);
                returned += (char)readByte[0];
            }

            return returned.Substring(0, returned.IndexOf("<EOF>"));
        }

        public void attemptToAuthenticate()
        {
            //TODO: Implement logic to authenticate user assuming the username and password have been read

            authenticated = true;
            reportLogInAttempt();
        }

        private void reportLogInAttempt()
        {
            ClientLogInAttempt attempt = new ClientLogInAttempt
            {
                username = this.username,
                password = this.password,
                clientAccepted = this.authenticated,
                requestSource = ((IPEndPoint)connection.RemoteEndPoint).Serialize()
            };

            authenticationEndpoint.SendLocal(attempt);
        }

       
    }

    
}
